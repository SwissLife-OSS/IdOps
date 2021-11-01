using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class ApiScopeService : TenantResourceService<ApiScope>, IApiScopeService
    {
        private readonly IApiScopeStore _apiScopeStore;
        private readonly IResourceManager<ApiScope> _resourceManager;

        public ApiScopeService(
            IApiScopeStore apiScopeStore,
            IResourceManager<ApiScope> resourceManager,
            IUserContextAccessor userContextAccessor)
                : base(userContextAccessor, apiScopeStore)
        {
            _apiScopeStore = apiScopeStore;
            _resourceManager = resourceManager;
        }

        public override async Task<IReadOnlyList<ApiScope>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants =
                await GetUserMergedTenantsAsync(tenants?.ToArray(), cancellationToken);

            return await _apiScopeStore.GetByTenantsAsync(ids, userTenants, cancellationToken);
        }

        public async Task<IReadOnlyList<ApiScope>> GetManyAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _apiScopeStore.GetByIdsAsync(ids, cancellationToken);
        }

        public async Task<ApiScope> SaveAsync(
            SaveApiScopeRequest request,
            CancellationToken cancellationToken)
        {
            ApiScope apiScope = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            apiScope.Tenant = request.Tenant;
            apiScope.Name = request.Name;
            apiScope.DisplayName = request.DisplayName;
            apiScope.ShowInDiscoveryDocument = request.ShowInDiscoveryDocument;
            apiScope.Enabled = request.Enabled;
            apiScope.Description = request.Description;

            SaveResourceResult<ApiScope> result = await _resourceManager
                .SaveAsync(apiScope, cancellationToken);

            return result.Resource;
        }

        public async Task<IReadOnlyList<IResource>> GetDependenciesAsync(
            ApiScope client,
            CancellationToken cancellationToken)
        {
            var dependencies = new List<IResource>();

            return dependencies;
        }
    }
}
