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
    public class ApplicationService : UserTenantService, IApplicationService
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IClientStore _clientStore;
        private readonly IResourceManager<Application> _resourceManager;
        private readonly IClientService _clientService;
        private readonly IClientTemplateService _clientTemplateService;
        private readonly IEnvironmentService _environmentService;

        public ApplicationService(
            IApplicationStore applicationStore,
            IClientStore clientStore,
            IUserContextAccessor userContextAccessor,
            IResourceManager<Application> resourceManager,
            IClientService clientService,
            IClientTemplateService clientTemplateService,
            IEnvironmentService environmentService)
            : base(userContextAccessor)
        {
            _applicationStore = applicationStore;
            _clientStore = clientStore;
            _resourceManager = resourceManager;
            _clientService = clientService;
            _clientTemplateService = clientTemplateService;
            _environmentService = environmentService;
        }

        public async Task<Application> UpdateAsync(
            UpdateApplicationRequest request,
            CancellationToken cancellationToken)
        {
            Application application = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            application.ApiScopes = request.ApiScopes.ToList();
            application.IdentityScopes = request.IdentityScopes.ToList();
            application.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            application.Name = request.Name;
            application.RedirectUris = request.RedirectUris.ToList();

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(application, cancellationToken);

            if (result.Diff is { } d && d.Any())
            {
                foreach (Guid clientId in application.ClientIds)
                {
                    Client client = await _clientService.GetClientByIdAsync(clientId, cancellationToken);
                    client = await _clientTemplateService.UpdateClientAsync(client, application, cancellationToken);

                    await _clientService.UpdateClientAsync(client, cancellationToken);
                }
            }

            return result.Resource;
        }

        public async Task<CreateAplicationResult> CreateAsync(
            CreateApplicationRequest request,
            CancellationToken cancellationToken)
        {
            Application application = _resourceManager.CreateNew();
            var createdClients = new List<CreatedClientInfo>();

            application.Name = request.Name;
            application.ApiScopes = request.ApiScopes.ToList();
            application.IdentityScopes = request.IdentityScopes.ToList();
            application.Tenant = request.Tenant;
            application.TemplateId = request.TemplateId;
            application.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            application.RedirectUris = request.RedirectUris.ToList();

            Application createdApplication = await CreateClientByEnvironmentAsync(
                _resourceManager,
                application,
                request.Environments,
                request.TemplateId,
                createdClients,
                cancellationToken);

            return new CreateAplicationResult(createdApplication, createdClients);
        }

        public Task<IReadOnlyList<Application>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            return _applicationStore.GetByIdsAsync(ids, cancellationToken);
        }

        public async Task<Application> GetByIdAsync(
            Guid id, CancellationToken cancellationToken)
        {
            Application result = await _applicationStore.GetByIdAsync(id, cancellationToken);
            return result;
        }

        public async Task<Application?> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
        {
            return await _applicationStore.GetByClientIdAsync(clientId, cancellationToken);
        }

        public async Task<SearchResult<Application>> SearchAsync(
            SearchApplicationsRequest request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants = await GetUserMergedTenantsAsync(request.Tenants, cancellationToken);
            request = request with { Tenants = userTenants };

            return await _applicationStore.SearchAsync(request, cancellationToken);
        }

        public async Task<Application> RemoveClientAsync(
            RemoveClientRequest request, CancellationToken cancellationToken)
        {
            Application application = await _resourceManager.GetExistingOrCreateNewAsync(
                    request.Id,
                    cancellationToken);

            application.ClientIds = application.ClientIds
                    .Where(x => x != request.ClientId)
                    .ToList();

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(application, cancellationToken);

            return result.Resource;
        }

        public async Task<Application> AddClientAsync(
            AddClientRequest request,
            CancellationToken cancellationToken)
        {
            Application application = await _resourceManager.GetExistingOrCreateNewAsync(
                    request.Id,
                    cancellationToken);

            application.ClientIds.Add(request.ClientId);

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(application, cancellationToken);

            return result.Resource;
        }

        public async Task<IEnumerable<Client>> SearchUnMappedClientsAsync(
            string tenant,
            CancellationToken cancellationToken)
        {
            Task<IReadOnlyList<Client>>? taskClients =
                _clientStore.SearchByTenantAsync(tenant, cancellationToken);
            Task<IReadOnlyList<Application>>? taskApps =
                _applicationStore.SearchByTenantAsync(tenant, cancellationToken);

            await Task.WhenAll(taskClients, taskApps);

            IEnumerable<Guid> allAssignedClientIds = taskApps.Result.SelectMany(c => c.ClientIds);

            IEnumerable<Client>? clients = taskClients.Result
                .Where(t => !allAssignedClientIds.Contains(t.Id));

            return clients.ToArray();
        }

        public async Task<Application> AddEnvironmentToApplicationAsnyc(
            AddEnvironmentToApplicationRequest request, CancellationToken cancellationToken)
        {
            Application application = await _resourceManager.GetExistingOrCreateNewAsync(
                request.Id,
                cancellationToken);

            var createdClients = new List<CreatedClientInfo>();

            return await CreateClientByEnvironmentAsync(
                _resourceManager,
                application,
                request.Environments,
                application.TemplateId,
                createdClients,
                cancellationToken);
        }

        public async Task<Application> CreateClientByEnvironmentAsync(
            IResourceManager<Application> manager,
            Application application,
            IEnumerable<Guid> environmentsOfApplication,
            Guid templateId,
            List<CreatedClientInfo> createdClients,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Model.Environment>? environments =
                await _environmentService.GetAllAsync(cancellationToken);

            foreach (Guid envId in environmentsOfApplication)
            {
                Model.Environment env = environments.Single(t => t.Id == envId);

                (Client client, string? secret) clientResult = await _clientTemplateService
                    .CreateClientAsync(templateId, env, application, cancellationToken);

                clientResult.client.Id = Guid.NewGuid();

                await _clientService.CreateClientAsync(clientResult.client, cancellationToken);

                application.ClientIds.Add(clientResult.client.Id);
                createdClients.Add(new CreatedClientInfo(
                    clientResult.client.Id,
                    clientResult.client.ClientId,
                    clientResult.client.Name)
                {
                    SecretValue = clientResult.secret
                });
            }

            SaveResourceResult<Application> result = await manager
                .SaveAsync(application, cancellationToken);

            return result.Resource;
        }
    }
}
