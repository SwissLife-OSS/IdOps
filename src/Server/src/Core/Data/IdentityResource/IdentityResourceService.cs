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
        private readonly IResourceManager<IdentityResource> _resourceManager;

        public IdentityResourceService(
            IdOpsServerOptions options,
            IIdentityResourceStore identityResourceStore,
            IResourceManager<IdentityResource> resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(options, userContextAccessor)
        {
            _identityResourceStore = identityResourceStore;
            _resourceManager = resourceManager;
        }

        public override async ValueTask<IdentityResource?> GetResourceByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            await _identityResourceStore.GetByIdAsync(id, cancellationToken);

        public override async Task<IReadOnlyList<IdentityResource>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            // TODO We should move this into the new permission model
            if (!UserContext.HasPermission(Permissions.ClientAuthoring.IdentityResource.Manage))
            {
                return Array.Empty<IdentityResource>();
            }

            IReadOnlyList<string> userTenants =
                await GetUserMergedTenantsAsync(tenants?.ToArray(), cancellationToken);

            return await _identityResourceStore
                .GetAllAsync(ids: ids, tenants: userTenants, cancellationToken);
        }

        public async Task<IdentityResource> SaveAsync(
            SaveIdentityResourceRequest request,
            CancellationToken cancellationToken)
        {
            IdentityResource resource =
                await _resourceManager.GetExistingOrCreateNewAsync(request.Id, cancellationToken);

            resource.IdentityServerGroupId = request.IdentityServerGroupId;
            resource.Tenants = request.Tenants.ToList();
            resource.Name = request.Name;
            resource.DisplayName = request.DisplayName;
            resource.Enabled = request.Enabled;
            resource.Description = request.Description;
            resource.UserClaims = request.UserClaims.ToList();
            resource.Required = request.Required;
            resource.Emphasize = request.Emphasize;

            SaveResourceResult<IdentityResource> result =
                await _resourceManager.SaveAsync(resource, cancellationToken);

            return result.Resource;
        }
    }
}
