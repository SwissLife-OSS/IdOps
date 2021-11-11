using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps.Templates
{
    public class ClientTemplateService : UserTenantService, IClientTemplateService
    {
        private readonly IClientTemplateStore _clientTemplateStore;
        private readonly IEnumerable<IClientIdGenerator> _clientIdGenerators;
        private readonly IEnvironmentService _environmentService;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly ISecretService _secretService;

        public ClientTemplateService(
            IClientTemplateStore clientTemplateStore,
            IEnumerable<IClientIdGenerator> clientIdGenerators,
            IEnvironmentService environmentService,
            ITemplateRenderer templateRenderer,
            ISecretService secretService,
            IUserContextAccessor userContextAccessor)
             : base(userContextAccessor)
        {
            _clientTemplateStore = clientTemplateStore;
            _clientIdGenerators = clientIdGenerators;
            _environmentService = environmentService;
            _templateRenderer = templateRenderer;
            _secretService = secretService;
        }

        public async Task<IEnumerable<ClientTemplate>> GetAllAsync(
            CancellationToken cancellationToken)
        {

            IReadOnlyList<string> tenants = await GetUserTenantsAsync(cancellationToken);

            return await _clientTemplateStore.GetAllAsync(tenants, cancellationToken);
        }

        public async Task<(Client client, string? secret)> CreateClientAsync(
            ClientTemplate template,
            Model.Environment environment,
            Application application,
            CancellationToken cancellationToken)
        {
            var client = new Client();

            client.Environments = new[] { environment.Id };
            client.Tenant = application.Tenant;
            client.ClientId = GetClientId(template);
            client.Name = GetName(template, application, environment);
            client.ClientUri = GetUri(template, application, environment)?.ToLower();
            client.RedirectUris = GetRedirectUris(template, application, client, environment);
            client.AllowedGrantTypes = GetGrantTypes(template, application);
            client.AllowedScopes = BuildScopes(application);
            client.AllowAccessTokensViaBrowser = template.AllowAccessTokensViaBrowser;
            client.AllowOfflineAccess = template.AllowOfflineAccess;
            client.EnabledProviders = template.EnabledProviders;
            client.DataConnectors = template.DataConnectors;
            client.RequirePkce = template.RequirePkce;
            client.RequireClientSecret = template.RequireClientSecret;

            if (client.RequireClientSecret)
            {
                (Secret secret, string? value) secret = await GetSecretAsync(
                    template,
                    environment,
                    cancellationToken);

                client.ClientSecrets = new List<Secret> { secret.secret };

                return (client, secret.value);
            }
            else
            {
                return (client, null);
            }
        }

        public async Task<Client> UpdateClientAsync(
            Client client,
            Application application,
            CancellationToken cancellationToken)
        {
            ClientTemplate? template = await GetByIdAsync(application.TemplateId, cancellationToken);
            Model.Environment environment = await _environmentService
                .GetByIdAsync(client.Environments.Single(), cancellationToken);

            // TODO: merge with client
            client.AllowedGrantTypes = GetGrantTypes(template, application);
            client.AllowedScopes = BuildScopes(application);
            client.AllowAccessTokensViaBrowser = template.AllowAccessTokensViaBrowser;
            client.RedirectUris = GetRedirectUris(template, application, client, environment);

            return client;
        }

        private IList<string> GetRedirectUris(
            ClientTemplate template,
            Application application,
            Client client,
            Model.Environment environment)
        {
            var uris = new HashSet<string>();

            foreach (var uri in template.RedirectUris.Concat(application.RedirectUris))
            {
                uris.Add(RenderTemplate(uri, application, environment, client));
            }

            return uris.ToList();
        }

        private IList<string> GetGrantTypes(ClientTemplate template, Application application)
        {
            return template.AllowedGrantTypes
                .Concat(application.AllowedGrantTypes)
                .Distinct()
                .ToList();
        }

        private async Task<(Secret, string?)> GetSecretAsync(
            ClientTemplate template,
            Model.Environment environment,
            CancellationToken cancellationToken)
        {
            var secrets = new Secret();

            ClientTemplateSecret? secretConfig = template.Secrets
                .FirstOrDefault(x => x.EnvironmentId == environment.Id);

            if (secretConfig is { } c && c.Value is string)
            {
                return (new Secret
                {
                    Type = secretConfig.Type,
                    Value = secretConfig.Value
                }, null);
            }
            else
            {
                (Secret, string) created = await _secretService.CreateSecretAsync(
                     new AddSecretRequest
                     {
                         Generator = template.SecretGenerator,
                         SaveValue = true
                     });

                return created;
            }
        }

        public async Task<(Client client, string? secret)> CreateClientAsync(
            Guid templateId,
            Model.Environment environment,
            Application application,
            CancellationToken cancellationToken)
        {
            ClientTemplate template = await GetByIdAsync(templateId, cancellationToken);

            return await CreateClientAsync(template, environment, application, cancellationToken);
        }

        private string? GetUri(
            ClientTemplate template,
            Application application,
            Model.Environment environment)
        {
            if (template.UrlTemplate is { })
            {
                return RenderTemplate(
                    template.UrlTemplate,
                    application,
                    environment);
            }

            return null;
        }

        private string GetName(
            ClientTemplate template,
            Application application,
            Model.Environment environment)
        {
            if (template.NameTemplate is { })
            {
                return RenderTemplate(
                    template.NameTemplate,
                    application,
                    environment);
            }
            else
            {
                return $"{environment.Name}_{application.Name}";
            }
        }

        private string GetClientId(ClientTemplate template)
        {
            IClientIdGenerator generator = _clientIdGenerators
                .FirstOrDefault(x => x.Name == template.ClientIdGenerator) ??
                _clientIdGenerators.First();

            return generator.CreateClientId();
        }

        private string RenderTemplate(
            string template,
            Application application,
            Model.Environment environment)
        {
            var data = new
            {
                environment = environment.Name,
                application = application.Name
            };

            return _templateRenderer.Render(template, data);
        }

        private string RenderTemplate(
            string template,
            Application application,
            Model.Environment environment,
            Client client)
        {
            var data = new
            {
                environment = environment.Name,
                application = application.Name,
                clientUri = client.ClientUri
            };

            return _templateRenderer.Render(template, data);
        }

        public Task<ClientTemplate> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _clientTemplateStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<ClientTemplate> SaveClientTemplate(
            SaveClientTemplateRequest input,
            CancellationToken cancellationToken)
        {
            ClientTemplate clientTemplate;

            List<ClientTemplateSecret>? hashedSecrets = new List<ClientTemplateSecret>();

            if (input.Id.HasValue)
            {
                clientTemplate = await GetByIdAsync(input.Id.Value, cancellationToken);

                foreach (ClientTemplateSecret? secret in input.Secrets)
                {
                    if (secret.Value is { } && secret.Value != "***")
                    {
                        ClientTemplateSecret? existing = clientTemplate!.Secrets
                            .FirstOrDefault(x => x.EnvironmentId == secret.EnvironmentId);

                        if (existing is { })
                        {
                            existing.Value = secret.Value.ToSha256();
                        }
                        else
                        {
                            hashedSecrets.Add(new ClientTemplateSecret
                            {
                                EnvironmentId = secret.EnvironmentId,
                                Value = secret.Value.ToSha256(),
                                Type = "SharedSecret"
                            });
                        }
                    }
                    else
                    {
                        hashedSecrets.Add(secret);
                    }
                }
            }
            else
            {
                clientTemplate = new ClientTemplate();
                clientTemplate.Id = Guid.NewGuid();
                clientTemplate.Secrets = clientTemplate.Secrets
                    .Where(x => x.Value is { })
                    .Select(x => new ClientTemplateSecret
                    {
                        EnvironmentId = x.EnvironmentId,
                        Value = x.Value.ToSha256(),
                        Type = "SharedSecret"
                    }).ToList();

            }

            clientTemplate.Name = input.Name;
            clientTemplate.RedirectUris = input.RedirectUris?.ToList() ?? new List<string>();
            clientTemplate.RequirePkce = input.RequirePkce;
            clientTemplate.Tenant = input.Tenant;
            clientTemplate.AllowedGrantTypes = input.AllowedGrantTypes.ToList();
            clientTemplate.ClientIdGenerator = input.ClientIdGenerator;
            clientTemplate.NameTemplate = input.NameTemplate;
            clientTemplate.UrlTemplate = input.UrlTemplate;
            clientTemplate.DataConnectors = input.DataConnectors?.ToList() ?? new List<DataConnectorOptions>();
            clientTemplate.EnabledProviders = input.EnabledProviders?.ToList() ?? new List<EnabledProvider>();
            clientTemplate.Secrets = hashedSecrets;
            clientTemplate.ApiScopes = input.ApiScopes?.ToList() ?? new List<Guid>();
            clientTemplate.IdentityScopes = input.IdentityScopes?.ToList() ?? new List<Guid>();

            await _clientTemplateStore.SaveClientTemplate(clientTemplate, cancellationToken);

            return clientTemplate;
        }

        private ICollection<ClientScope> BuildScopes(
            Application application)
        {
            List<ClientScope>? scopes = new List<ClientScope>();

            scopes.AddRange(application.ApiScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Resource,
                Id = x
            }));

            scopes.AddRange(application.IdentityScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Identity,
                Id = x
            }));

            return scopes;
        }
    }
}
