using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Store
{
    public class ResourceStore : IResourceStore, IManageResourceStore
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IApiScopeRepository _apiScopeRepository;
        private readonly IIdentityResourceRepository _identityResourceRepository;
        private readonly IUserClaimRuleRepository _userClaimRuleRepository;
        private readonly IPersonalAccessTokenRepository _personalAccessTokenRepository;

        public ResourceStore(
            IApiResourceRepository apiResourceRepository,
            IApiScopeRepository apiScopeRepository,
            IIdentityResourceRepository identityResourceRepository,
            IUserClaimRuleRepository userClaimRuleRepository,
            IPersonalAccessTokenRepository personalAccessTokenRepository)
        {
            _apiResourceRepository = apiResourceRepository;
            _apiScopeRepository = apiScopeRepository;
            _identityResourceRepository = identityResourceRepository;
            _userClaimRuleRepository = userClaimRuleRepository;
            _personalAccessTokenRepository = personalAccessTokenRepository;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(
            IEnumerable<string> apiResourceNames)
        {
            return await _apiResourceRepository.GetByNameAsync(apiResourceNames, default);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _apiResourceRepository.GetByScopeNameAsync(scopeNames, default);
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _apiScopeRepository.GetByNameAsync(scopeNames, default);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(
            IEnumerable<string> scopeNames)
        {
            return await _identityResourceRepository.GetByNameAsync(scopeNames, default);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            Task<IEnumerable<IdOpsIdentityResource>>? idResTask = _identityResourceRepository
                .GetAllAsync(default);

            Task<IEnumerable<ApiScope>>? apiScopeTask = _apiScopeRepository.GetAllAsync(default);

            Task<IEnumerable<IdOpsApiResource>>? apiResTask = _apiResourceRepository
                .GetAllAsync(default);

            await Task.WhenAll(idResTask, apiResTask, apiScopeTask);

            return new Resources(idResTask.Result, apiResTask.Result, apiScopeTask.Result);
        }

        public async Task<UpdateResourceResult> UpdateIdentityResourceAsync(
            IdOpsIdentityResource resource,
            CancellationToken cancellationToken)
        {
            return await _identityResourceRepository.UpdateAsync(resource, cancellationToken);
        }

        public async Task<UpdateResourceResult> UpdateApiResourceAsync(
            IdOpsApiResource resource,
            CancellationToken cancellationToken)
        {
            return await _apiResourceRepository.UpdateAsync(resource, cancellationToken);
        }

        public async Task<UpdateResourceResult> UpdateApiScopeAsync(
            IdOpsApiScope scope,
            CancellationToken cancellationToken)
        {
            return await _apiScopeRepository.UpdateAsync(scope, cancellationToken);
        }

        public async Task<UpdateResourceResult> UpdateUserClaimRuleAsync(
            UserClaimRule rule,
            CancellationToken cancellationToken)
        {
            return await _userClaimRuleRepository.UpdateAsync(rule, cancellationToken);
        }

        public async Task<UpdateResourceResult> UpdatePersonalAccessTokenAsync(
            IdOpsPersonalAccessToken token,
            CancellationToken cancellationToken)
        {
            return await _personalAccessTokenRepository
                .UpdateResourceAsync(token, cancellationToken);
        }
    }
}
