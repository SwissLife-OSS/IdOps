using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Model;
using IpFilterPolicy = IdOps.Model.IpFilterPolicy;

namespace IdOps
{
    public class ClientMessageFactory : TenantResourceMessageFactory<Client>
    {
        public ClientMessageFactory(IIdentityServerGroupService identityServerGroupService)
            : base(identityServerGroupService)
        {
        }

        public override ValueTask<IdOpsResource?> BuildPublishMessage(
            IPublishingContext context,
            Client client,
            CancellationToken cancellationToken) =>
            new(new IdOpsClient
            {
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AccessTokenType = PublisherHelper.MapEnum<Duende.IdentityServer.Models.AccessTokenType>(client.AccessTokenType),
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = client.AllowedCorsOrigins,
                AllowedGrantTypes = client.AllowedGrantTypes,
                AllowedIdentityTokenSigningAlgorithms =
                    client.AllowedIdentityTokenSigningAlgorithms,
                AllowOfflineAccess = client.AllowOfflineAccess,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowedScopes = context.GetAllowedScopes(client.AllowedScopes),
                AllowRememberConsent = client.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                Claims = client.Claims?
                    .Select(x => new Duende.IdentityServer.Models.ClientClaim
                    {
                        Type = x.Type, Value = x.Value, ValueType = x.ValueType
                    })
                    .ToArray() ?? Array.Empty<Duende.IdentityServer.Models.ClientClaim>(),
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                ClientId = client.ClientId,
                ClientName = client.Name,
                ClientSecrets = client.ClientSecrets?
                    .Select(s => new Duende.IdentityServer.Models.Secret(s.Value, s.Description, s.Expiration))
                    .ToArray() ?? Array.Empty<Duende.IdentityServer.Models.Secret>(),
                ClientUri = client.ClientUri,
                ConsentLifetime = client.ConsentLifetime,
                Description = client.Description,
                DataConnectors = PublisherHelper.MapDataConnectors(client.DataConnectors),
                DeviceCodeLifetime = client.DeviceCodeLifetime,
                DisplayName = client.Description,
                Enabled = client.Enabled,
                EnabledProviders = PublisherHelper.MapProviders(client.EnabledProviders),
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                IncludeJwtId = client.IncludeJwtId,
                EnableLocalLogin = true,
                IdentityProviderRestrictions = new List<string>(),
                LogoUri = client.LogoUri,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                PostLogoutRedirectUris = client.PostLogoutRedirectUris,
                ProtocolType = client.ProtocolType,
                RedirectUris = client.RedirectUris,
                RefreshTokenExpiration =
                    PublisherHelper.MapEnum<Duende.IdentityServer.Models.TokenExpiration>(client.RefreshTokenExpiration),
                RefreshTokenUsage = PublisherHelper.MapEnum<Duende.IdentityServer.Models.TokenUsage>(client.RefreshTokenUsage),
                RequireClientSecret = client.RequireClientSecret,
                RequireConsent = client.RequireConsent,
                RequirePkce = client.RequirePkce,
                RequireRequestObject = client.RequireRequestObject,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = client.UserCodeType,
                UserSsoLifetime = client.UserSsoLifetime,
                Properties = client.Properties,
                Tenant = client.Tenant,
                IpAddressFilter = new IdentityServer.Model.IpAddressFilter
                {
                    WarnOnly = client.IpAddressFilter.WarnOnly,
                    AllowList = client.IpAddressFilter.AllowList,
                    Policy = client.IpAddressFilter.Policy switch {
                      IpFilterPolicy.Internal => IdentityServer.Model.IpFilterPolicy.Internal,
                      IpFilterPolicy.AllowList => IdentityServer.Model.IpFilterPolicy.AllowList,
                      IpFilterPolicy.Public => IdentityServer.Model.IpFilterPolicy.Public,
                      _ => throw new InvalidOperationException(
                          $"IpFilterPolicy was out of range: {client.IpAddressFilter.Policy}")
                    }
                }
            });
    }
}
