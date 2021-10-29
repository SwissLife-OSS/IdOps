using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Store;

namespace IdOps
{
    public class ApiResourceService : TenantResourceService<ApiResource>, IApiResourceService
    {
        private readonly IResourceManager<ApiResource> _resourceManager;
        private readonly ISecretService _secretService;
        private readonly IApiResourceStore _apiResourceStore;
        private readonly IApiScopeStore _apiScopeStore;

        public ApiResourceService(
            IUserContextAccessor userContextAccessor,
            IResourceManager<ApiResource> resourceManager,
            ISecretService secretService,
            IApiResourceStore apiResourceStore,
            IApiScopeStore apiScopeStore)
                : base(userContextAccessor, apiResourceStore)
        {
            _resourceManager = resourceManager;
            _secretService = secretService;
            _apiResourceStore = apiResourceStore;
            _apiScopeStore = apiScopeStore;
        }

        public async Task<IReadOnlyList<ApiResource>> GetByTenantAsync(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants = await GetUserTenantsAsync(cancellationToken);

            return await _apiResourceStore.GetByTenantsAsync(null, userTenants, cancellationToken);
        }

        public async Task<ApiResource> SaveAsync(
            SaveApiResourceRequest request,
            CancellationToken cancellationToken)
        {
            ApiResource resource = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            resource.Tenant = request.Tenant;
            resource.Name = request.Name;
            resource.DisplayName = request.DisplayName;
            resource.Enabled = request.Enabled;
            resource.Description = request.Description;
            resource.Scopes = request.Scopes.ToList();

            SaveResourceResult<ApiResource> result = await _resourceManager.SaveAsync(
                resource,
                cancellationToken);

            return result.Resource;
        }

        public async Task<(ApiResource, string)> AddSecretAsync(
            AddApiSecretRequest request,
            CancellationToken cancellationToken)
        {
            ApiResource apiResource = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            (Secret secret, string secretValue) = await _secretService.CreateSecretAsync(request);

            apiResource.ApiSecrets.Add(secret);

            SaveResourceResult<ApiResource> result = await _resourceManager
                .SaveAsync(apiResource, cancellationToken);

            return (result.Resource, secretValue);
        }

        public async Task<ApiResource> RemoveSecretAsync(
            RemoveApiSecretRequest request,
            CancellationToken cancellationToken)
        {
            ApiResource apiResource = await _resourceManager.GetExistingOrCreateNewAsync(
                request.ApiResourceId,
                cancellationToken);

            apiResource.ApiSecrets = apiResource.ApiSecrets
                .Where(x => x.Id != request.Id)
                .ToList();

            SaveResourceResult<ApiResource> result = await _resourceManager
                .SaveAsync(apiResource, cancellationToken);

            return result.Resource;
        }

        public async Task<IReadOnlyList<IResource>> GetDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            ApiResource resource = await _apiResourceStore.GetByIdAsync(id, cancellationToken);

            return await GetDependenciesAsync(resource, cancellationToken);
        }

        public async Task<IReadOnlyList<IResource>> GetDependenciesAsync(
            ApiResource apiResource,
            CancellationToken cancellationToken)
        {
            var dependencies = new List<IResource>();

            IReadOnlyList<ApiScope> scopes =
                await _apiScopeStore.GetByIdsAsync(apiResource.Scopes, cancellationToken);

            dependencies.AddRange(scopes);

            return dependencies;
        }
    }
}
