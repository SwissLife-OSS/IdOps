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
    public class ClientService : TenantResourceService<Client>, IClientService
    {
        private readonly IClientStore _clientStore;
        private readonly IResourceManager<Client> _resourceManager;
        private readonly ISecretService _secretService;
        private readonly IDictionary<string, IClientIdGenerator> _clientIdGenerators;
        private readonly string[] _clientIdGeneratorsNames;
        private readonly string[] _sharedSecretGeneratorNames;

        public ClientService(
            IdOpsServerOptions options,
            IClientStore clientStore,
            IUserContextAccessor userContextAccessor,
            IResourceManager<Client> resourceManager,
            IEnumerable<IClientIdGenerator> clientIdGenerators,
            ISecretService secretService,
            IEnumerable<ISharedSecretGenerator> sharedSecretGenerators)
            : base(options, userContextAccessor, clientStore)
        {
            _clientStore = clientStore;
            _resourceManager = resourceManager;
            _secretService = secretService;
            _clientIdGenerators = clientIdGenerators.ToDictionary(x => x.Name);
            _clientIdGeneratorsNames = _clientIdGenerators.Keys.ToArray();
            _sharedSecretGeneratorNames = sharedSecretGenerators.Select(x => x.Name).ToArray();
        }

        public async Task<Client> GetClientByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _clientStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<Client>> GetManyAsync(
            IReadOnlyList<Guid> ids,
            CancellationToken cancellationToken)
        {
            return await _clientStore.GetByIdsAsync(ids, cancellationToken);
        }

        public async Task<SearchResult<Client>> SearchClientsAsync(
            SearchClientsRequest request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> tenants = await GetUserTenantsAsync(cancellationToken);
            request = request with
            {
                Tenants = tenants.Intersect(request.Tenants ?? Array.Empty<string>())
            };
            return await _clientStore.SearchAsync(request, cancellationToken);
        }

        public async Task<Client> CreateClientAsync(
            Client client,
            CancellationToken cancellationToken)
        {
            _resourceManager.SetNewVersion(client);

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(client, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> CreateClientAsync(
            CreateClientRequest request,
            CancellationToken cancellationToken)
        {
            Client client = _resourceManager.CreateNew();

            client.Tenant = request.Tenant;
            client.Environments = request.Environments.ToList();
            client.Name = request.Name;
            client.AllowedGrantTypes = request.AllowedGrantTypes?.ToList();
            client.AllowedScopes = BuildScopes(request.ApiScopes, request.IdentityScopes);

            if (request.ClientId is { })
            {
                client.ClientId = request.ClientId;
            }
            else if (request.ClientIdGenerator is { } generatorId &&
                _clientIdGenerators.TryGetValue(generatorId, out IClientIdGenerator? generator))
            {
                client.ClientId = generator.CreateClientId();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(request));
            }

            SaveResourceResult<Client> result =
                await _resourceManager.SaveAsync(client, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> UpdateClientAsync(
            Client client,
            CancellationToken cancellationToken)
        {
            await _resourceManager.GetExistingOrCreateNewAsync(client.Id, cancellationToken);

            SaveResourceResult<Client> result = await _resourceManager.SaveAsync(client, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> UpdateClientAsync(
            UpdateClientRequest request,
            CancellationToken cancellationToken)
        {
            Client client =
                await _resourceManager.GetExistingOrCreateNewAsync(request.Id, cancellationToken);

            client.Name = request.Name;
            client.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            client.Environments = request.Environments?.ToList() ?? new List<Guid>();
            client.AllowedScopes = BuildScopes(request.ApiScopes, request.IdentityScopes);
            client.AllowedCorsOrigins = request.AllowedCorsOrigins;
            client.RedirectUris = request.RedirectUris;
            client.Tenant = request.Tenant;
            client.RequirePkce = request.RequirePkce;
            client.RequireClientSecret = request.RequireClientSecret;
            client.AllowPlainTextPkce = request.AllowPlainTextPkce;
            client.AllowOfflineAccess = request.AllowOfflineAccess;
            client.AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser;

            client.IdentityTokenLifetime = request.IdentityTokenLifetime;
            client.AccessTokenLifetime = request.AccessTokenLifetime;
            client.AuthorizationCodeLifetime = request.AuthorizationCodeLifetime;
            client.AbsoluteRefreshTokenLifetime = request.AbsoluteRefreshTokenLifetime;
            client.SlidingRefreshTokenLifetime = request.SlidingRefreshTokenLifetime;
            client.ConsentLifetime = request.ConsentLifetime;
            client.RefreshTokenExpiration = request.RefreshTokenExpiration;
            client.UserSsoLifetime = request.UserSsoLifetime;
            client.DeviceCodeLifetime = request.DeviceCodeLifetime;
            client.AccessTokenType = request.AccessTokenType;

            client.Description = request.Description;
            client.ClientUri = request.ClientUri;
            client.LogoUri = request.LogoUri;
            client.RequireConsent = request.RequireConsent;
            client.AllowRememberConsent = request.AllowRememberConsent;
            client.RequireRequestObject = request.RequireRequestObject;
            client.AlwaysIncludeUserClaimsInIdToken = request.AlwaysIncludeUserClaimsInIdToken;
            client.UpdateAccessTokenClaimsOnRefresh = request.UpdateAccessTokenClaimsOnRefresh;
            client.AlwaysSendClientClaims = request.AlwaysSendClientClaims;
            client.ClientClaimsPrefix = request.ClientClaimsPrefix;

            client.PostLogoutRedirectUris = request.PostLogoutRedirectUris;
            client.FrontChannelLogoutUri = request.FrontChannelLogoutUri;
            client.FrontChannelLogoutSessionRequired = request.FrontChannelLogoutSessionRequired;
            client.BackChannelLogoutUri = request.BackChannelLogoutUri;
            client.BackChannelLogoutSessionRequired = request.BackChannelLogoutSessionRequired;
            client.Properties = request.Properties?.ToDictionary(k => k.Key, v => v.Value);
            client.Claims = request.Claims ?? new List<ClientClaim>();
            client.DataConnectors = request.DataConnectors?.ToList();
            client.EnabledProviders = request.EnabledProviders?.ToList();

            SaveResourceResult<Client> result = await _resourceManager.SaveAsync(client, cancellationToken);

            return result.Resource;
        }

        public async Task<(Client, string)> AddClientSecretAsync(
            AddClientSecretRequest request,
            CancellationToken cancellationToken)
        {
            Client client =
                await _resourceManager.GetExistingOrCreateNewAsync( request.Id, cancellationToken);

            (Secret secret, string secretValue) = await _secretService.CreateSecretAsync(request);

            client.ClientSecrets.Add(secret);

            SaveResourceResult<Client> result = await _resourceManager.SaveAsync(client, cancellationToken);

            return (result.Resource, secretValue);
        }

        public async Task<Client> RemoveClientSecretAsync(
            RemoveClientSecretRequest request,
            CancellationToken cancellationToken)
        {
            Client client =
                await _resourceManager.GetExistingOrCreateNewAsync(request.ClientId, cancellationToken);

            client.ClientSecrets = client.ClientSecrets?
                .Where(x => x.Id != request.Id)
                .ToList();

            SaveResourceResult<Client> result = await _resourceManager.SaveAsync(client, cancellationToken);

            return result.Resource;
        }

        private static ICollection<ClientScope> BuildScopes(
            IReadOnlyList<Guid> apiScopes,
            IReadOnlyList<Guid> identityScopes)
        {
            List<ClientScope> scopes = new();
            scopes.AddRange(apiScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Resource, Id = x
            }));

            scopes.AddRange(identityScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Identity, Id = x
            }));

            return scopes;
        }

        public IReadOnlyList<string> GetClientIdGenerators() => _clientIdGeneratorsNames;

        public IReadOnlyList<string> GetSharedSecretGenerators() => _sharedSecretGeneratorNames;

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
