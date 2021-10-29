using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Store.Mongo;
using MongoDB.Driver;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace IdOps
{
    public class ResourceImporter
    {
        private readonly IIdOpsDbContext _dbContext ;
        private const string RootDir = @"C:\Temp\contoso-export";

        public ResourceImporter(IIdOpsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        ResourceVersion DefaultVersion => new ResourceVersion
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Import",
            Version = 0
        };

        string DefaultTenant = "Contoso";
        Guid defaultGroup = Guid.Parse("6832dcbd-f348-458b-9fab-dbbe3a7ad41e");

        public async Task CreateApplications(CancellationToken cancellationToken)
        {
            List<Client> clients = await _dbContext.Clients.AsQueryable().ToListAsync();

            var groups = clients
                .Where(x => x.Name.Contains("_"))
                .Select(x => new
                {
                    Name = x.Name.Split('_').LastOrDefault(),
                    Client = x,
                }).GroupBy(x => x.Name);

            foreach (var group in groups)
            {
                Client? refClient = group
                    .Where(x => x.Client.Name.StartsWith("A_"))
                    .Select(x => x.Client)
                    .FirstOrDefault();

                if (refClient != null)
                {
                    var app = new Application
                    {
                        Id = Guid.NewGuid(),
                        TemplateId = Guid.Parse("4d86c243-4180-4279-8f85-45d4f3543122"),
                        Name = group.Key,
                        AllowedGrantTypes = refClient.AllowedGrantTypes,
                        ApiScopes = refClient.AllowedScopes.Where(x => x.Type == ScopeType.Resource)
                            .Select(x => x.Id)
                            .ToList(),
                        IdentityScopes = refClient.AllowedScopes.Where(x => x.Type == ScopeType.Identity)
                            .Select(x => x.Id)
                            .ToList(),
                        Version = ResourceVersion.CreateNew("Import"),
                        ClientIds = group.Select(x => x.Client.Id).ToList(),
                        Tenant = refClient.Tenant
                    };

                    await _dbContext.Applications.InsertOneAsync(app);
                }
            }
        }

        public async Task ImportAsync(CancellationToken cancellationToken)
        {
            IEnumerable<ApiScope> apiScopes = GetResources<ApiScope>("api_scope.json");
            IEnumerable<ApiResourceImport> apiResources = GetResources<ApiResourceImport>("api_resource.json");
            IEnumerable<IdentityResource> identityResources = GetResources<IdentityResource>("identity_resource.json");
            //IEnumerable<GrantType> grantTypes = GetResources<GrantType>("grant_types.json");

            foreach (ApiScope scope in apiScopes)
            {
                scope.Tenant = scope.Tenant ?? DefaultTenant;
                scope.Id = Guid.NewGuid();
                scope.Version = DefaultVersion;
            }

            List<ApiResource> newApiResources = new();

            foreach (ApiResourceImport apiRes in apiResources)
            {
                var newApiRes = new ApiResource();
                newApiRes.InjectFrom(apiRes);
                newApiRes.Version = DefaultVersion;
                newApiRes.Tenant = apiRes.Tenant ?? DefaultTenant;
                newApiRes.Id = Guid.NewGuid();

                newApiRes.Scopes = apiScopes
                    .Where(x => apiRes.Scopes.Contains(x.Name))
                    .Select(x => x.Id)
                    .ToList();

                foreach (Secret secret in apiRes.ApiSecrets)
                {
                    secret.Id = Guid.NewGuid();
                }

                newApiResources.Add(newApiRes);
            }

            foreach (IdentityResource idRes in identityResources)
            {
                idRes.IdentityServerGroupId = defaultGroup;
                idRes.Tenants = new[] { DefaultTenant };
                idRes.Id = Guid.NewGuid();
                idRes.Version = DefaultVersion;
            }

            //await _dbContext.GrantTypes.InsertManyAsync(grantTypes);
            await _dbContext.ApiScopes.InsertManyAsync(apiScopes);
            await _dbContext.ApiResources.InsertManyAsync(newApiResources);
            await _dbContext.IdentityResources.InsertManyAsync(identityResources);
        }

        private IEnumerable<T> GetResources<T>(string filename)
        {
            var json = File.ReadAllText(Path.Join(RootDir, filename));
            IEnumerable<T> resources = JsonConvert.DeserializeObject<IEnumerable<T>>(json);

            return resources;
        }

        public async Task ImportClientsAsync()
        {
            List<ApiScope>? apiScopes = await _dbContext.ApiScopes.AsQueryable().ToListAsync();
            List<IdentityResource>? identityResources = await _dbContext.IdentityResources.AsQueryable().ToListAsync();
            List<Model.Environment>? environments = await _dbContext.Environments.AsQueryable().ToListAsync();

            IEnumerable<ClientImport> importClients = GetResources<ClientImport>("identityClients.json");

            //Model.Environment environment = environments.Single(x => x.Name == environmentName);

            var clients = new List<Client>();

            foreach (ClientImport importClient in importClients)
            {
                var client = new Client();
                client.InjectFrom(importClient);

                client.Id = Guid.NewGuid();
                client.Tenant = importClient.Tenant;
                client.Name = importClient.ClientName;

                client.Environments = environments
                    .Where(x => importClient.Environments.Contains(x.Name))
                    .Select(x => x.Id).ToList();

                client.Version = DefaultVersion;

                foreach (Secret secret in client.ClientSecrets)
                {
                    secret.Id = Guid.NewGuid();
                }
                var scopes = new List<ClientScope>();
                scopes.AddRange(apiScopes
                    .Where(s => importClient.AllowedScopes.Contains(s.Name) && s.Tenant == client.Tenant)
                    .Select(x => new ClientScope { Id = x.Id, Type = ScopeType.Resource }));

                scopes.AddRange(identityResources
                    .Where(s => importClient.AllowedScopes.Contains(s.Name) && s.IdentityServerGroupId == defaultGroup)
                    .Select(x => new ClientScope { Id = x.Id, Type = ScopeType.Identity }));

                client.AllowedScopes = scopes;

                clients.Add(client);
            }

            List<Client> existing = await _dbContext.Clients.AsQueryable().ToListAsync();

            IEnumerable<string> newClientIds = clients.Select(c => c.ClientId);
            IEnumerable<Client>? existingClientIds = existing
                .Where(x => newClientIds.Contains(x.ClientId));

            if (existingClientIds.Count() == 0)
            {
                await _dbContext.Clients.InsertManyAsync(clients);

            }
            //clients.RemoveAll(x => existing.Select(c => c.ClientId).Contains(x.ClientId));
        }
    }
}
