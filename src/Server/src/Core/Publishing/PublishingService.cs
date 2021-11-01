using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;
using Environment = IdOps.Model.Environment;

namespace IdOps
{
    public class PublishingService : UserTenantService, IPublishingService
    {
        private readonly IResourcePublishStateStore _resourcePublishStateStore;
        private readonly IApprovalService _approvalService;
        private readonly IEnvironmentService _environmentService;
        private readonly IIdentityServerService _identityServerService;
        private readonly IResourceServiceResolver _resourceServiceResolver;

        public PublishingService(
            IResourcePublishStateStore resourcePublishStateStore,
            IApprovalService approvalService,
            IEnvironmentService environmentService,
            IIdentityServerService identityServerService,
            IResourceServiceResolver resourceServiceResolver,
            IUserContextAccessor userContextAccessor)
            : base(userContextAccessor)
        {
            _resourcePublishStateStore = resourcePublishStateStore;
            _approvalService = approvalService;
            _environmentService = environmentService;
            _identityServerService = identityServerService;
            _resourceServiceResolver = resourceServiceResolver;
        }

        public async IAsyncEnumerable<PublishedResource> GetPublishedResourcesAsync(
            PublishedResourcesRequest? filter,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IReadOnlyList<Guid>? ids = filter is { ResourceId: { Count: >0 } }
                ? filter.ResourceId
                : null;

            IReadOnlyList<ResourceApproval> approvalStates =
                await GetApprovalStatesAsync(filter, cancellationToken);

            IReadOnlyList<ResourcePublishState>? publishedStates =
                await GetResourcePublishStatesAsync(ids, cancellationToken);

            IReadOnlyList<Environment> environments =
                await GetEnvironmentsAsync(filter, cancellationToken);

            IReadOnlyList<string> tenants =
                await GetUserMergedTenantsAsync(filter?.Tenants, cancellationToken);

            IdentityServerGroup? serverGroup = await GetServerGroupAsync(filter, cancellationToken);

            PublishedResourceMatcher matcher =
                new(publishedStates, approvalStates, environments, serverGroup);

            IReadOnlyList<string> resourceTypes =
                filter?.ResourceTypes?.Count > 0
                    ? filter.ResourceTypes
                    : _resourceServiceResolver.AvailableResourceTypes;

            foreach (var resourceType in resourceTypes)
            {
                if (!_resourceServiceResolver.TryResolveService(resourceType,
                    out IResourceService? service))
                {
                    continue;
                }

                IReadOnlyList<IResource> resources =
                    await service.GetByTenantsAsync(ids, tenants, cancellationToken);

                foreach (var resource in resources)
                {
                    if (matcher.TryMatchPublishedResources(service,
                        resource,
                        out PublishedResource? publishedResource))
                    {
                        yield return publishedResource;
                    }
                }
            }
        }

        private async Task<IdentityServerGroup?> GetServerGroupAsync(
            PublishedResourcesRequest? filter,
            CancellationToken cancellationToken)
        {
            IdentityServerGroup? serverGroup = null;
            if (filter is { IdentityServerGroupId: { } })
            {
                serverGroup = await _identityServerService
                    .GetGroupByIdAsync(filter.IdentityServerGroupId.Value, cancellationToken);
            }

            return serverGroup;
        }

        private async Task<IReadOnlyList<Environment>> GetEnvironmentsAsync(
            PublishedResourcesRequest? filter,
            CancellationToken cancellationToken)
        {
            IEnumerable<Environment> environments =
                await _environmentService.GetAllAsync(cancellationToken);

            if (filter is { Environment: { } })
            {
                environments = environments.Where(x => x.Id == filter.Environment);
            }

            return environments.ToList();
        }

        private async Task<IReadOnlyList<ResourcePublishState>> GetResourcePublishStatesAsync(
            IReadOnlyList<Guid>? ids,
            CancellationToken cancellationToken)
        {
            IEnumerable<ResourcePublishState> result = ids is { }
                ? await _resourcePublishStateStore.GetManyAsync(ids, cancellationToken)
                : await _resourcePublishStateStore.GetAllAsync(cancellationToken);

            return result.ToList();
        }

        private async Task<IReadOnlyList<ResourceApproval>> GetApprovalStatesAsync(
            PublishedResourcesRequest? filter,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<ResourceApproval> approvalStates = Array.Empty<ResourceApproval>();
            if (filter is null)
            {
                return approvalStates;
            }

            ResourceApprovalRequest? openApprovalRequest = new()
            {
                Tenants = filter.Tenants,
                EnvironmentId = filter.Environment,
                ResourceIds = filter.ResourceId,
                IdentityServerGroupId = filter.IdentityServerGroupId,
                ResourceTypes = filter.ResourceTypes,
            };

            return await _approvalService
                .GetResourceApprovals(openApprovalRequest, cancellationToken)
                .ToListAsync(cancellationToken);
        }
    }
}
