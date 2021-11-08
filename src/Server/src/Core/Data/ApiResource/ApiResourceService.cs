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
    public class ApiResourceService : TenantResourceService<ApiResource>, IApiResourceService
    {
        private readonly IResourceManager _resourceManager;
        private readonly ISecretService _secretService;
        private readonly IApiResourceStore _apiResourceStore;

        public ApiResourceService(
            IdOpsServerOptions options,
            IUserContextAccessor userContextAccessor,
            IResourceManager resourceManager,
            ISecretService secretService,
            IApiResourceStore apiResourceStore)
                : base(options, userContextAccessor, apiResourceStore)
        {
            _resourceManager = resourceManager;
            _secretService = secretService;
            _apiResourceStore = apiResourceStore;
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
            ResourceChangeContext<ApiResource> context = await _resourceManager
                .GetExistingOrCreateNewAsync<ApiResource>(request.Id, cancellationToken);

            context.Resource.Tenant = request.Tenant;
            context.Resource.Name = request.Name;
            context.Resource.DisplayName = request.DisplayName;
            context.Resource.Enabled = request.Enabled;
            context.Resource.Description = request.Description;
            context.Resource.Scopes = request.Scopes.ToList();

            SaveResourceResult<ApiResource> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<(ApiResource, string)> AddSecretAsync(
            AddApiSecretRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<ApiResource> context = await _resourceManager
                .GetExistingOrCreateNewAsync<ApiResource>(request.Id, cancellationToken);

            (Secret secret, string secretValue) = await _secretService.CreateSecretAsync(request);

            context.Resource.ApiSecrets.Add(secret);

            SaveResourceResult<ApiResource> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return (result.Resource, secretValue);
        }

        public async Task<ApiResource> RemoveSecretAsync(
            RemoveApiSecretRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<ApiResource> context = await _resourceManager
                .GetExistingOrCreateNewAsync<ApiResource>(
                request.ApiResourceId,
                cancellationToken);

            context.Resource.ApiSecrets = context.Resource.ApiSecrets
                .Where(x => x.Id != request.Id)
                .ToList();

            SaveResourceResult<ApiResource> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public override bool IsAllowedToPublish()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.Publish);
        }

        public override bool IsAllowedToApprove()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.Approve);
        }
    }
}
