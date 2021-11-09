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
    public class ApiScopeService : TenantResourceService<ApiScope>, IApiScopeService
    {
        private readonly IApiScopeStore _apiScopeStore;
        private readonly IResourceManager _resourceManager;

        public ApiScopeService(
            IdOpsServerOptions options,
            IApiScopeStore apiScopeStore,
            IResourceManager resourceManager,
            IUserContextAccessor userContextAccessor)
                : base(options, userContextAccessor, apiScopeStore)
        {
            _apiScopeStore = apiScopeStore;
            _resourceManager = resourceManager;
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
            ResourceChangeContext<ApiScope> context = await _resourceManager
                .GetExistingOrCreateNewAsync<ApiScope>(request.Id, cancellationToken);

            context.Resource.Tenant = request.Tenant;
            context.Resource.Name = request.Name;
            context.Resource.DisplayName = request.DisplayName;
            context.Resource.ShowInDiscoveryDocument = request.ShowInDiscoveryDocument;
            context.Resource.Enabled = request.Enabled;
            context.Resource.Description = request.Description;

            SaveResourceResult<ApiScope> result = await _resourceManager
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
