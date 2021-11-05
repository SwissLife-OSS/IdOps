using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public class InMemoryPublishingContext
        : IPublishingContext
    {
        private readonly IResourceStores _stores;
        private readonly Dictionary<Guid, ApiResource> _apiResources;
        private readonly Dictionary<Guid, IdentityResource> _identityResources;
        private readonly Dictionary<Guid, ApiScope> _apiScopes;
        private readonly Dictionary<Guid, Model.Environment> _environments;

        public InMemoryPublishingContext(
            IResourceStores stores,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiScope> apiScopes,
            IEnumerable<Model.Environment> environments,
            Guid environmentId)
        {
            _stores = stores;
            EnvironmentId = environmentId;
            _apiResources = apiResources.ToDictionary(x => x.Id);
            _identityResources = identityResources.ToDictionary(x => x.Id);
            _apiScopes = apiScopes.ToDictionary(x => x.Id);
            _environments = environments.ToDictionary(x => x.Id);
        }

        public Guid EnvironmentId { get; }

        public ApiResource? GetApiResourceById(Guid id)
        {
            _apiResources.TryGetValue(id, out ApiResource? result);
            return result;
        }

        public IdentityResource? GetIdentityResourceById(Guid id)
        {
            _identityResources.TryGetValue(id, out IdentityResource? result);
            return result;
        }

        public ApiScope? GetApiScopeById(Guid id)
        {
            _apiScopes.TryGetValue(id, out ApiScope? result);
            return result;
        }

        public Model.Environment? GetEnvironmentById(Guid id)
        {
            _environments.TryGetValue(id, out Model.Environment? result);
            return result;
        }

        public Task<ICollection<string>> GetClientIdsOfApplicationsAsync(
            IEnumerable<Guid> applicationIds,
            CancellationToken cancellationToken)
        {
            return GetClientIdsOfApplicationsAsync(applicationIds, default, cancellationToken);
        }

        public async Task<ICollection<string>> GetClientIdsOfApplicationsAsync(
            IEnumerable<Guid> applicationIds,
            Guid environmentId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Application> apps =
                await _stores.Applications.GetByIdsAsync(applicationIds, cancellationToken);

            Guid[] appIds = apps.SelectMany(x => x.ClientIds).ToArray();

            IEnumerable<Client> clients =
                await _stores.Clients.GetByIdsAsync(appIds, cancellationToken);

            if (environmentId != default)
            {
                clients = clients.Where(x => x.Environments.Contains(environmentId));
            }

            return clients
                .Select(x => x.ClientId)
                .Distinct()
                .ToArray();
        }
    }
}
