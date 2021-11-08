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
        private readonly IResourceManager _resourceManager;
        private readonly IClientService _clientService;
        private readonly IClientTemplateService _clientTemplateService;
        private readonly IEnvironmentService _environmentService;

        public ApplicationService(
            IApplicationStore applicationStore,
            IClientStore clientStore,
            IUserContextAccessor userContextAccessor,
            IResourceManager resourceManager,
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
            ResourceChangeContext<Application> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Application>(request.Id, cancellationToken);

            context.Resource.ApiScopes = request.ApiScopes.ToList();
            context.Resource.IdentityScopes = request.IdentityScopes.ToList();
            context.Resource.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            context.Resource.Name = request.Name;
            context.Resource.RedirectUris = request.RedirectUris.ToList();

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            if (result.Diff is { } d && d.Any())
            {
                foreach (Guid clientId in context.Resource.ClientIds)
                {
                    Client client = await _clientService.GetClientByIdAsync(clientId, cancellationToken);
                    client = await _clientTemplateService.UpdateClientAsync(client, context.Resource, cancellationToken);

                    await _clientService.UpdateClientAsync(client, cancellationToken);
                }
            }

            return result.Resource;
        }

        public async Task<CreateAplicationResult> CreateAsync(
            CreateApplicationRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Application> context = _resourceManager.CreateNew<Application>();
            var createdClients = new List<CreatedClientInfo>();

            context.Resource.Name = request.Name;
            context.Resource.ApiScopes = request.ApiScopes.ToList();
            context.Resource.IdentityScopes = request.IdentityScopes.ToList();
            context.Resource.Tenant = request.Tenant;
            context.Resource.TemplateId = request.TemplateId;
            context.Resource.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            context.Resource.RedirectUris = request.RedirectUris.ToList();

            Application createdApplication = await CreateClientByEnvironmentAsync(
                _resourceManager,
                context, 
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

        public async Task<Application?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _applicationStore.GetByIdAsync(id, cancellationToken);
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
            ResourceChangeContext<Application> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Application>(request.Id, cancellationToken);

            context.Resource.ClientIds = context.Resource.ClientIds
                    .Where(x => x != request.ClientId)
                    .ToList();

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<Application> AddClientAsync(
            AddClientRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Application> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Application>(request.Id, cancellationToken);

            context.Resource.ClientIds.Add(request.ClientId);

            SaveResourceResult<Application> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

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
            ResourceChangeContext<Application> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Application>(request.Id, cancellationToken);

            var createdClients = new List<CreatedClientInfo>();

            return await CreateClientByEnvironmentAsync(
                _resourceManager,
                context,
                request.Environments,
                context.Resource.TemplateId,
                createdClients,
                cancellationToken);
        }

        public async Task<Application> CreateClientByEnvironmentAsync(
            IResourceManager manager,
            ResourceChangeContext<Application> context,
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
                    .CreateClientAsync(templateId, env, context.Resource, cancellationToken);

                clientResult.client.Id = Guid.NewGuid();

                await _clientService.CreateClientAsync(clientResult.client, cancellationToken);

                context.Resource.ClientIds.Add(clientResult.client.Id);
                createdClients.Add(new CreatedClientInfo(
                    clientResult.client.Id,
                    clientResult.client.ClientId,
                    clientResult.client.Name)
                {
                    SecretValue = clientResult.secret
                });
            }

            SaveResourceResult<Application> result = await manager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }
    }
}
