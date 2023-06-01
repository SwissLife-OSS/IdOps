using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public interface ITenantInput
    {
        string Tenant { get; }
    }

    public record UpdateClientRequest(
        Guid Id,
        string Name,
        string Tenant,
        IReadOnlyList<string> AllowedGrantTypes,
        IReadOnlyList<Guid> ApiScopes,
        IReadOnlyList<Guid> IdentityScopes,
        IReadOnlyList<Guid> Environments) : ITenantInput
    {
        public string? ProtocolType { get; init; }

        public bool RequireClientSecret { get; init; }

        public string? Description { get; init; }

        public string? ClientUri { get; init; }

        public string? LogoUri { get; init; }

        public bool RequireConsent { get; init; }

        public bool AllowRememberConsent { get; init; }

        public bool RequirePkce { get; init; } = true;

        public bool AllowPlainTextPkce { get; init; }

        public bool RequireRequestObject { get; init; }
        
        public bool AllowTokenGeneration { get; init; }

        public bool AllowAccessTokensViaBrowser { get; init; }

        public ICollection<string> RedirectUris { get; init; }

        public ICollection<string> PostLogoutRedirectUris { get; init; }

        public string? FrontChannelLogoutUri { get; init; }

        public bool FrontChannelLogoutSessionRequired { get; init; }

        public string? BackChannelLogoutUri { get; init; }

        public bool BackChannelLogoutSessionRequired { get; init; }

        public bool AllowOfflineAccess { get; init; } = false;

        public bool AlwaysIncludeUserClaimsInIdToken { get; init; }

        public int IdentityTokenLifetime { get; init; } = 300;

        public ICollection<string> AllowedIdentityTokenSigningAlgorithms { get; init; }

        public int AccessTokenLifetime { get; init; }

        public int AuthorizationCodeLifetime { get; init; }

        public int AbsoluteRefreshTokenLifetime { get; init; }

        public int SlidingRefreshTokenLifetime { get; init; }

        public int? ConsentLifetime { get; init; } = null;

        public TokenUsage RefreshTokenUsage { get; init; }

        public bool UpdateAccessTokenClaimsOnRefresh { get; init; }

        public TokenExpiration RefreshTokenExpiration { get; init; }

        public AccessTokenType AccessTokenType { get; init; }

        public bool AlwaysSendClientClaims { get; init; }

        public string? ClientClaimsPrefix { get; init; }

        public string? PairWiseSubjectSalt { get; init; }

        public int? UserSsoLifetime { get; init; }

        public string? UserCodeType { get; init; }

        public int DeviceCodeLifetime { get; init; }

        public ICollection<string> AllowedCorsOrigins { get; init; }

        public List<ClientProperty>? Properties { get; init; }

        public List<ClientClaim>? Claims { get; init; }

        public List<DataConnectorOptions>? DataConnectors { get; init; }

        public List<EnabledProvider>? EnabledProviders { get; init; }
    }

    public record ClientProperty(string Key, string Value);

    public record AddClientSecretRequest(Guid Id)
        : AddSecretRequest
    {
    }

    public record AddApiSecretRequest(Guid Id)
    : AddSecretRequest
    {
    }


    public record AddSecretRequest
    {
        public string? Generator { get; init; }

        public string? Value { get; init; }

        public string? Name { get; init; }

        public bool? SaveValue { get; init; }
    }

    public record RemoveClientSecretRequest(Guid ClientId, Guid Id);
    
    public record RemoveApiSecretRequest(Guid ApiResourceId, Guid Id);
}
