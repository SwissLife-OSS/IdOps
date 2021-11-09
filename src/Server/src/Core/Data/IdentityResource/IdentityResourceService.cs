using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Configuration;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class IdentityResourceService
        : ResourceService<IdentityResource>, IIdentityResourceService
    {
        private readonly IIdentityResourceStore _identityResourceStore;
        private readonly IResourceManager _resourceManager;

        public IdentityResourceService(
            IdOpsServerOptions options,
            IIdentityResourceStore identityResourceStore,
            IResourceManager resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(options, userContextAccessor, identityResourceStore)
        {
            _identityResourceStore = identityResourceStore;
            _resourceManager = resourceManager;
        }

        public override async Task<IReadOnlyList<IdentityResource>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants =
                await GetUserMergedTenantsAsync(tenants?.ToArray(), cancellationToken);

            return await _identityResourceStore
                .GetAllAsync(ids: ids, tenants: userTenants, cancellationToken);
        }

        public async Task<IdentityResource> SaveAsync(
            SaveIdentityResourceRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<IdentityResource> context = await _resourceManager
                .GetExistingOrCreateNewAsync<IdentityResource>(request.Id, cancellationToken);

            context.Resource.IdentityServerGroupId = request.IdentityServerGroupId;
            context.Resource.Tenants = request.Tenants.ToList();
            context.Resource.Name = request.Name;
            context.Resource.DisplayName = request.DisplayName;
            context.Resource.Enabled = request.Enabled;
            context.Resource.Description = request.Description;
            context.Resource.UserClaims = request.UserClaims.ToList();
            context.Resource.Required = request.Required;
            context.Resource.Emphasize = request.Emphasize;

            SaveResourceResult<IdentityResource> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public override bool IsAllowedToPublish()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.IdentityResource.Manage);
        }

        public override bool IsAllowedToApprove()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.IdentityResource.Manage);
        }
    }
}
