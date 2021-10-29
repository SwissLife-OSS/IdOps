using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Store;

namespace IdOps
{
    public class ClientDependencyResolver : ResourceDependencyResolver<Client>
    {
        private readonly IClientService _clientService;
        private readonly IApplicationService _applicationService;
        private readonly IApiScopeStore _apiScopeStore;
        private readonly IIdentityResourceStore _identityResourceStore;
        private readonly IUserClaimRulesService _userClaimRulesService;

        public ClientDependencyResolver(
            IClientService clientService,
            IApplicationService applicationService,
            IApiScopeStore apiScopeStore,
            IIdentityResourceStore identityResourceStore,
            IUserClaimRulesService userClaimRulesService)
        {
            _clientService = clientService;
            _applicationService = applicationService;
            _apiScopeStore = apiScopeStore;
            _identityResourceStore = identityResourceStore;
            _userClaimRulesService = userClaimRulesService;
        }

        public override async Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            var dependencies = new List<IResource>();

            Client client = await _clientService.GetClientByIdAsync(id, cancellationToken);
            dependencies.AddRange(await GetApiScopesAsync(client.AllowedScopes, cancellationToken));
            dependencies.AddRange(await GetIdentityScopesAsync(client.AllowedScopes, cancellationToken));

            dependencies.AddRange(await GetRulesAsync(client, cancellationToken));

            return dependencies;
        }

        private async Task<IEnumerable<IResource>> GetApiScopesAsync(
            ICollection<ClientScope> allowedScopes,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid> resourceScopeIds = allowedScopes
                .Where(s => s.Type == ScopeType.Resource)
                .Select(s => s.Id);

            return await _apiScopeStore.GetByIdsAsync(resourceScopeIds, cancellationToken);
        }

        private async Task<IEnumerable<IResource>> GetIdentityScopesAsync(
            ICollection<ClientScope> allowedScopes,
            CancellationToken cancellationToken)
        {
            IEnumerable<Guid> identityScopeIds = allowedScopes
                .Where(s => s.Type == ScopeType.Identity)
                .Select(s => s.Id);

            return await _identityResourceStore.GetByIdsAsync(identityScopeIds, cancellationToken);
        }

        private async Task<IEnumerable<IResource>> GetRulesAsync(
            Client client,
            CancellationToken cancellationToken)
        {
            Application? application = await _applicationService
                .GetByClientIdAsync(client.Id, cancellationToken);

            if (application == null)
            {
                return Enumerable.Empty<IResource>();
            }

            return await _userClaimRulesService
                .GetByApplicationAsync(application.Id, cancellationToken);
        }
    }
}
