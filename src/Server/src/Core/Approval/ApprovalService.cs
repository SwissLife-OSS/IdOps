using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;
using Microsoft.Azure.Amqp.Framing;

namespace IdOps
{
    public class ApprovalService : UserTenantService, IApprovalService
    {
        private readonly IResourceApprovalStateStore _resourceApprovalStateStore;
        private readonly IResourceApprovalLogStore _resourceApprovalLogStore;
        private readonly IEnvironmentService _environmentService;
        private readonly IIdentityServerService _identityServerService;
        private readonly IResourceServiceResolver _resourceServiceResolver;

        public ApprovalService(
            IResourceApprovalStateStore resourceApprovalStateStore,
            IResourceApprovalLogStore resourceApprovalLogStore,
            IEnvironmentService environmentService,
            IIdentityServerService identityServerService,
            IResourceServiceResolver resourceServiceResolver,
            IUserContextAccessor userContextAccessor)
            : base(userContextAccessor)
        {
            _resourceApprovalStateStore = resourceApprovalStateStore;
            _resourceApprovalLogStore = resourceApprovalLogStore;
            _resourceServiceResolver = resourceServiceResolver;
            _environmentService = environmentService;
            _identityServerService = identityServerService;
        }

        public async IAsyncEnumerable<ResourceApproval> GetResourceApprovals(
            ResourceApprovalRequest? filter,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IReadOnlyList<ResourceApprovalState> approvalStates =
                Array.Empty<ResourceApprovalState>();

            IReadOnlyList<Guid>? ids = null;

            if (filter is { ResourceIds: { Count: > 0 } resourceIds })
            {
                ids = resourceIds;
            }

            if (ids is { })
            {
                approvalStates =
                    await _resourceApprovalStateStore.GetManyAsync(ids, cancellationToken);
            }
            else
            {
                approvalStates =
                    await _resourceApprovalStateStore.GetAllAsync(cancellationToken);
            }

            IReadOnlyList<Model.Environment> environments =
                await _environmentService.GetAllAsync(cancellationToken);

            if (filter is { EnvironmentId: { } })
            {
                environments = environments.Where(x => x.Id == filter.EnvironmentId).ToArray();
            }

            IReadOnlyList<string> tenants =
                await GetUserMergedTenantsAsync(filter?.Tenants, cancellationToken);

            if (filter is { Tenants: { } t })
            {
                tenants = tenants.Intersect(t).ToArray();
            }

            IdentityServerGroup? serverGroup = null;
            if (filter is { IdentityServerGroupId: { } })
            {
                serverGroup = await _identityServerService
                    .GetGroupByIdAsync(filter.IdentityServerGroupId.Value, cancellationToken);
            }

            ResourceApprovalStateLookup stateLookup = new(approvalStates);

            IReadOnlyList<string> resourceTypes =
                filter?.ResourceTypes?.Count > 0
                    ? filter.ResourceTypes
                    : _resourceServiceResolver.AvailableResourceTypes;

            foreach (var resourceType in resourceTypes)
            {
                if (!_resourceServiceResolver.TryResolveService(resourceType, out IResourceService? service))
                {
                    continue;
                }

                IReadOnlyList<IResource> resources =
                    await service.GetByTenantsAsync(ids, tenants, cancellationToken);

                foreach (var resource in resources)
                {
                    if (service.RequiresApproval(resource.Id) &&
                        TryMatchOpenResourceApprovals(
                            stateLookup,
                            serverGroup,
                            environments,
                            service,
                            resource,
                            out ResourceApproval? approval))
                    {
                        yield return approval;
                    }
                }
            }
        }


        public async Task<ApproveResourcesResult> ApproveResourcesAsync(
            ApproveResourcesRequest request,
            CancellationToken cancellationToken)
        {
            IUserContext userContext = _userContextAccessor.Context ??
                throw CouldNotAccessUserContextException.New();

            // TODO: Replace with proper permission management once it is in place
            if (!userContext.HasPermission(Permissions.ClientAuthoring.Approve))
            {
                return ApproveResourcesResult.Empty;
            }

            List<Guid> response = new();
            foreach (ApproveResource resource in request.Resources)
            {
                if (_resourceServiceResolver
                        .TryResolveService(resource.Type, out IResourceService? service) &&
                    service.RequiresApproval(resource.ResourceId))
                {
                    ResourceApprovalState state = new()
                    {
                        ApprovedAt = DateTime.Now,
                        EnvironmentId = resource.EnvironmentId,
                        Id = Guid.NewGuid(),
                        ResourceId = resource.ResourceId,
                        ResourceType = resource.Type,
                        Version = resource.Version
                    };
                    ResourceApprovalLog log = new()
                    {
                        ApprovedAt = state.ApprovedAt,
                        EnvironmentId = state.EnvironmentId,
                        ErrorMessage = "",
                        Operation = "Approve",
                        Id = Guid.NewGuid(),
                        RequestedBy = userContext.User.Name,
                        ResourceId = state.ResourceId,
                        ResourceType = state.ResourceType,
                        Version = state.Version,
                    };
                    await _resourceApprovalStateStore.SaveAsync(state, cancellationToken);
                    await _resourceApprovalLogStore.CreateAsync(log, cancellationToken);
                    response.Add(resource.ResourceId);
                }
            }

            return new ApproveResourcesResult(response);
        }

        private bool TryMatchOpenResourceApprovals(
            ResourceApprovalStateLookup approvalStateLookup,
            IdentityServerGroup? serverGroup,
            IEnumerable<Model.Environment> possibleEnvironments,
            IResourceService resourceService,
            IResource resource,
            [NotNullWhen(true)] out ResourceApproval? resourceApproval)
        {
            resourceApproval = null;
            if (serverGroup is { } && !resource.IsInServerGroup(serverGroup))
            {
                return false;
            }

            IEnumerable<Model.Environment> environments = resource.HasEnvironments()
                ? possibleEnvironments.Where(e => resource.GetEnvironmentIds().Contains(e.Id))
                : possibleEnvironments;

            resourceApproval = new ResourceApproval(
                resource.Id,
                resource.Title,
                resourceService.ResourceType,
                resource.Version);

            foreach (Model.Environment? environment in environments)
            {
                ResourceApprovalEnvironment approval;
                Guid id = resource.Id;

                if (approvalStateLookup
                    .TryGetEnvironment(id, environment, out ResourceApprovalState? state))
                {
                    approval =
                        ResourceApprovalEnvironment.From(
                            environment.Id,
                            state?.Version,
                            state?.ApprovedAt,
                            GetState(resource.Version, state));
                }
                else
                {
                    approval = ResourceApprovalEnvironment.NotApproved(environment.Id);
                }

                resourceApproval.Environments.Add(approval);
            }

            return true;
        }

        private string GetState(ResourceVersion version, ResourceApprovalState? envState)
        {
            if (envState is { })
            {
                if (envState.Version == version.Version!)
                {
                    return "Latest";
                }

                return "Outdated";
            }

            return "NotPublished";
        }

        private class ResourceApprovalStateLookup
        {
            private record struct ResourceLookup(Guid ResourceId, Guid Environment);

            private readonly ILookup<ResourceLookup, ResourceApprovalState> _lookup;
            private readonly HashSet<Guid> _resourceIds;

            public ResourceApprovalStateLookup(IReadOnlyList<ResourceApprovalState> publishedStates)
            {
                _lookup = publishedStates
                    .ToLookup(x => new ResourceLookup(x.ResourceId, x.EnvironmentId));

                _resourceIds = publishedStates.Select(x => x.ResourceId).ToHashSet();
            }

            public bool Contains(Guid resourceId) => _resourceIds.Contains(resourceId);

            public bool TryGetEnvironment(
                Guid resourceId,
                Model.Environment environment,
                out ResourceApprovalState? state)
            {
                state = _lookup[new ResourceLookup(resourceId, environment.Id)].FirstOrDefault();
                return state is not null;
            }
        }
    }
}
