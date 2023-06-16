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
        private readonly IResourceManager _resourceManager;
        private readonly ISecretService _secretService;
        private readonly IDictionary<string, IClientIdGenerator> _clientIdGenerators;
        private readonly string[] _clientIdGeneratorsNames;
        private readonly string[] _sharedSecretGeneratorNames;

        public ClientService(
            IdOpsServerOptions options,
            IClientStore clientStore,
            IUserContextAccessor userContextAccessor,
            IResourceManager resourceManager,
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
            ResourceChangeContext<Client> context = _resourceManager.SetNewVersion(client);

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> CreateClientAsync(
            CreateClientRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Client> context = _resourceManager.CreateNew<Client>();

            context.Resource.Tenant = request.Tenant;
            context.Resource.Environments = request.Environments.ToList();
            context.Resource.Name = request.Name;
            context.Resource.AllowedGrantTypes = request.AllowedGrantTypes?.ToList();
            context.Resource.AllowedScopes = BuildScopes(request.ApiScopes, request.IdentityScopes);
            context.Resource.IpAddressFilter = new IpAddressFilter
            {
                WarnOnly = false,
                AllowList = new List<string>(),
                Policy = IpFilterPolicy.Internal
            };

            if (request.ClientId is { })
            {
                context.Resource.ClientId = request.ClientId;
            }
            else if (request.ClientIdGenerator is { } generatorId &&
                _clientIdGenerators.TryGetValue(generatorId, out IClientIdGenerator? generator))
            {
                context.Resource.ClientId = generator.CreateClientId();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(request));
            }

            SaveResourceResult<Client> result =
                await _resourceManager.SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> UpdateClientAsync(
            Client client,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Client> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Client>(client.Id, cancellationToken);

            context.Resource.AllowedGrantTypes = client.AllowedGrantTypes;
            context.Resource.AllowedScopes = client.AllowedScopes;
            context.Resource.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
            context.Resource.RedirectUris = client.RedirectUris;

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<Client> UpdateClientAsync(
            UpdateClientRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Client> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Client>(request.Id, cancellationToken);

            context.Resource.Name = request.Name;
            context.Resource.AllowedGrantTypes = request.AllowedGrantTypes.ToList();
            context.Resource.Environments = request.Environments?.ToList() ?? new List<Guid>();
            context.Resource.AllowedScopes = BuildScopes(request.ApiScopes, request.IdentityScopes);
            context.Resource.AllowedCorsOrigins = request.AllowedCorsOrigins;
            context.Resource.RedirectUris = request.RedirectUris;
            context.Resource.Tenant = request.Tenant;
            context.Resource.RequirePkce = request.RequirePkce;
            context.Resource.RequireClientSecret = request.RequireClientSecret;
            context.Resource.AllowPlainTextPkce = request.AllowPlainTextPkce;
            context.Resource.AllowOfflineAccess = request.AllowOfflineAccess;
            context.Resource.AllowAccessTokensViaBrowser = request.AllowAccessTokensViaBrowser;

            context.Resource.IdentityTokenLifetime = request.IdentityTokenLifetime;
            context.Resource.AccessTokenLifetime = request.AccessTokenLifetime;
            context.Resource.AuthorizationCodeLifetime = request.AuthorizationCodeLifetime;
            context.Resource.AbsoluteRefreshTokenLifetime = request.AbsoluteRefreshTokenLifetime;
            context.Resource.SlidingRefreshTokenLifetime = request.SlidingRefreshTokenLifetime;
            context.Resource.ConsentLifetime = request.ConsentLifetime;
            context.Resource.RefreshTokenExpiration = request.RefreshTokenExpiration;
            context.Resource.RefreshTokenUsage = request.RefreshTokenUsage;
            context.Resource.UserSsoLifetime = request.UserSsoLifetime;
            context.Resource.DeviceCodeLifetime = request.DeviceCodeLifetime;
            context.Resource.AccessTokenType = request.AccessTokenType;

            context.Resource.Description = request.Description;
            context.Resource.ClientUri = request.ClientUri;
            context.Resource.LogoUri = request.LogoUri;
            context.Resource.RequireConsent = request.RequireConsent;
            context.Resource.AllowRememberConsent = request.AllowRememberConsent;
            context.Resource.RequireRequestObject = request.RequireRequestObject;
            context.Resource.AlwaysIncludeUserClaimsInIdToken = request.AlwaysIncludeUserClaimsInIdToken;
            context.Resource.UpdateAccessTokenClaimsOnRefresh = request.UpdateAccessTokenClaimsOnRefresh;
            context.Resource.AlwaysSendClientClaims = request.AlwaysSendClientClaims;
            context.Resource.ClientClaimsPrefix = request.ClientClaimsPrefix;

            context.Resource.PostLogoutRedirectUris = request.PostLogoutRedirectUris;
            context.Resource.FrontChannelLogoutUri = request.FrontChannelLogoutUri;
            context.Resource.FrontChannelLogoutSessionRequired = request.FrontChannelLogoutSessionRequired;
            context.Resource.BackChannelLogoutUri = request.BackChannelLogoutUri;
            context.Resource.BackChannelLogoutSessionRequired = request.BackChannelLogoutSessionRequired;
            context.Resource.Properties = request.Properties?.ToDictionary(k => k.Key, v => v.Value);
            context.Resource.Claims = request.Claims ?? new List<ClientClaim>();
            context.Resource.DataConnectors = request.DataConnectors?.ToList();
            context.Resource.EnabledProviders = request.EnabledProviders?.ToList();
            context.Resource.IpAddressFilter = request.IpAddressFilter;

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        public async Task<(Client, string)> AddClientSecretAsync(
            AddClientSecretRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Client> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Client>(request.Id, cancellationToken);

            (Secret secret, string secretValue) = await _secretService.CreateSecretAsync(request);

            context.Resource.ClientSecrets.Add(secret);

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return (result.Resource, secretValue);
        }

        public async Task<Client> RemoveClientSecretAsync(
            RemoveClientSecretRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<Client> context = await _resourceManager
                .GetExistingOrCreateNewAsync<Client>(request.ClientId, cancellationToken);

            context.Resource.ClientSecrets = context.Resource.ClientSecrets
                .Where(x => x.Id != request.Id)
                .ToList();

            SaveResourceResult<Client> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return result.Resource;
        }

        private static ICollection<ClientScope> BuildScopes(
            IReadOnlyList<Guid> apiScopes,
            IReadOnlyList<Guid> identityScopes)
        {
            List<ClientScope> scopes = new();
            scopes.AddRange(apiScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Resource,
                Id = x
            }));

            scopes.AddRange(identityScopes.Select(x => new ClientScope
            {
                Type = ScopeType.Identity,
                Id = x
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
