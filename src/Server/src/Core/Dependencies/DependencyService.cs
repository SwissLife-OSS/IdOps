using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;
using IdOps.Templates;

namespace IdOps
{
    // TODO: We could use IResourceDependencyResolver
    public class DependencyService : UserTenantService, IDependencyService
    {
        private readonly IClientService _clientService;
        private readonly IApplicationService _applicationService;
        private readonly IClientTemplateService _clientTemplateService;
        private readonly IIdentityResourceStore _identityResourceStore;
        private readonly IApiScopeStore _apiScopeStore;
        private readonly IApiResourceStore _apiResourceStore;

        public DependencyService(
            IClientService clientService,
            IApplicationService applicationService,
            IClientTemplateService clientTemplateService,
            IUserContextAccessor userContextAccessor,
            IIdentityResourceStore identityResourceStore,
            IApiScopeStore apiScopeStore,
            IApiResourceStore apiResourceStore)
                : base(userContextAccessor)
        {
            _clientService = clientService;
            _applicationService = applicationService;
            _clientTemplateService = clientTemplateService;
            _identityResourceStore = identityResourceStore;
            _apiScopeStore = apiScopeStore;
            _apiResourceStore = apiResourceStore;
        }

        public async Task<Dependency> GetAllDependenciesAsync(
            GetDependenciesRequest request,
            CancellationToken cancellationToken) =>
            request.Type switch
            {
                nameof(Client) => await GetAllClientDependencies(request.Id, cancellationToken),

                nameof(ApiResource) =>
                    await GetAllApiResourceDependencies(request.Id, cancellationToken),

                nameof(Application) =>
                    await GetAllApplicationDependencies(request.Id, cancellationToken),

                nameof(ClientTemplate) =>
                    await GetAllClientTemplateDependencies(request.Id, cancellationToken),

                _ => new Dependency()
            };

        public async Task<Dependency> GetAllClientDependencies(Guid id, CancellationToken cancellationToken)
        {
            Client? client = await _clientService.GetByIdAsync(id, cancellationToken);

            Dependency allDependencies = await GetScopeDependencies(
                client.AllowedScopes.Where(x => x.Type == ScopeType.Resource).Select(x => x.Id),
                client.AllowedScopes.Where(x => x.Type == ScopeType.Identity).Select(x => x.Id),
                client.Tenant,
                cancellationToken);

            return allDependencies;
        }

        public async Task<Dependency> GetAllApiResourceDependencies(Guid id, CancellationToken cancellationToken)
        {
            ApiResource apiResource = await _apiResourceStore.GetByIdAsync(id, cancellationToken);

            ICollection<ApiScope> apiScopes = new List<ApiScope>();
            if (apiResource.Scopes.Count > 0)
            {
                foreach (Guid scopeId in apiResource.Scopes)
                {
                    ApiScope apiScope = await _apiScopeStore.GetByIdAsync(scopeId, cancellationToken);
                    apiScopes.Add(apiScope);
                }
            }

            var dependencies = new Dependency();
            dependencies.ApiScopes = apiScopes;

            return dependencies;
        }

        public async Task<Dependency> GetAllApplicationDependencies(Guid id, CancellationToken cancellationToken)
        {
            Application application = await _applicationService.GetByIdAsync(id, cancellationToken);

            Dependency allDependencies = await GetScopeDependencies(
                application.ApiScopes,
                application.IdentityScopes,
                application.Tenant,
                cancellationToken);

            return allDependencies;
        }

        public async Task<Dependency> GetAllClientTemplateDependencies(Guid id, CancellationToken cancellationToken)
        {
            ClientTemplate template = await _clientTemplateService.GetByIdAsync(id, cancellationToken);

            Dependency allDependencies = await GetScopeDependencies(
                template.ApiScopes,
                template.IdentityScopes,
                template.Tenant,
                cancellationToken);

            return allDependencies;
        }

        public async Task<Dependency> GetScopeDependencies(
            IEnumerable<Guid> apiScopes,
            IEnumerable<Guid> identityResources,
            string tenant,
            CancellationToken cancellationToken)
        {
            var allDependencies = new Dependency();

            IReadOnlyList<ApiScope> apiScopeList = Array.Empty<ApiScope>();

            if (apiScopes != null)
            {
                apiScopeList = await _apiScopeStore.GetByIdsAsync( apiScopes, cancellationToken);
            }

            IReadOnlyList<IdentityResource> identityResourceList = Array.Empty<IdentityResource>();

            if (apiScopes != null)
            {
                identityResourceList =
                    await _identityResourceStore.GetByIdsAsync(identityResources, cancellationToken);
            }

            IReadOnlyList<string> tenantList = new[]
            {
                tenant
            };

            IEnumerable<ApiResource> allApiResources =
                await _apiResourceStore.GetByTenantsAsync(null, tenantList, cancellationToken);

            List<ApiResource> apiResourceList = new();

            foreach (ApiScope apiScope in apiScopeList)
            {
                ApiResource? element = allApiResources.FirstOrDefault(x => x.Scopes.Contains(apiScope.Id));
                if (element != null && !apiResourceList.Contains(element))
                {
                    apiResourceList.Add(element);
                }
            }

            allDependencies.ApiScopes = apiScopeList;
            allDependencies.IdentityResources = identityResourceList;
            allDependencies.ApiResources = apiResourceList;

            return allDependencies;
        }
    }
}
