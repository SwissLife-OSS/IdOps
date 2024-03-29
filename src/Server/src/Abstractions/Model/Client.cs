using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdOps.Model
{
    public class Client : ITenantResource
    {
        public Guid Id { get; set; }

        public string Tenant { get; set; }

        /// <summary>
        /// Specifies if client is enabled (defaults to <c>true</c>)
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Unique ID of the client
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the protocol type.
        /// </summary>
        /// <value>
        /// The protocol type.
        /// </value>
        public string? ProtocolType { get; set; } = "oidc";

        /// <summary>
        /// Client secrets - only relevant for flows that require a secret
        /// </summary>
        public ICollection<Secret>? ClientSecrets { get; set; } = new HashSet<Secret>();

        /// <summary>
        /// If set to false, no client secret is needed to request tokens at the token endpoint (defaults to <c>true</c>)
        /// </summary>
        public bool RequireClientSecret { get; set; } = true;
        
        /// <summary>
        /// If set to false, no token generation is allowed for this client (defaults to <c>false</c>)
        /// </summary>
        public bool AllowTokenGeneration { get; set; } = false;

        /// <summary>
        /// Client display name (used for logging and consent screen)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the client.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URI to further information about client (used on consent screen)
        /// </summary>
        public string? ClientUri { get; set; }

        /// <summary>
        /// URI to client logo (used on consent screen)
        /// </summary>
        public string? LogoUri { get; set; }

        /// <summary>
        /// Specifies whether a consent screen is required (defaults to <c>false</c>)
        /// </summary>
        public bool RequireConsent { get; set; } = false;

        /// <summary>
        /// Specifies whether user can choose to store consent decisions (defaults to <c>true</c>)
        /// </summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>
        /// Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit, Hybrid, ResourceOwner, ClientCredentials).
        /// </summary>
        public ICollection<string>? AllowedGrantTypes { get; set; } = new HashSet<string>();

        /// <summary>
        /// Specifies whether a proof key is required for authorization code based token requests (defaults to <c>true</c>).
        /// </summary>
        public bool RequirePkce { get; set; } = true;

        /// <summary>
        /// Specifies whether a proof key can be sent using plain method (not recommended and defaults to <c>false</c>.)
        /// </summary>
        public bool AllowPlainTextPkce { get; set; } = false;

        /// <summary>
        /// Specifies whether the client must use a request object on authorize requests (defaults to <c>false</c>.)
        /// </summary>
        public bool RequireRequestObject { get; set; } = false;

        /// <summary>
        /// Controls whether access tokens are transmitted via the browser for this client (defaults to <c>false</c>).
        /// This can prevent accidental leakage of access tokens when multiple response types are allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if access tokens can be transmitted via the browser; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAccessTokensViaBrowser { get; set; } = false;

        /// <summary>
        /// Specifies allowed URIs to return tokens or authorization codes to
        /// </summary>
        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        /// <summary>
        /// Specifies allowed URIs to redirect to after logout
        /// </summary>
        public ICollection<string> PostLogoutRedirectUris { get; set; } = new HashSet<string>();

        /// <summary>
        /// Specifies logout URI at client for HTTP front-channel based logout.
        /// </summary>
        public string? FrontChannelLogoutUri { get; set; }

        /// <summary>
        /// Specifies if the user's session id should be sent to the FrontChannelLogoutUri. Defaults to <c>true</c>.
        /// </summary>
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>
        /// Specifies logout URI at client for HTTP back-channel based logout.
        /// </summary>
        public string? BackChannelLogoutUri { get; set; }

        /// <summary>
        /// Specifies if the user's session id should be sent to the BackChannelLogoutUri. Defaults to <c>true</c>.
        /// </summary>
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [allow offline access]. Defaults to <c>false</c>.
        /// </summary>
        public bool AllowOfflineAccess { get; set; } = false;

        /// <summary>
        /// Specifies the api scopes that the client is allowed to request. If empty, the client can't access any scope
        /// </summary>
        public ICollection<ClientScope> AllowedScopes { get; set; } = new List<ClientScope>();

        /// <summary>
        /// When requesting both an id token and access token, should the user claims always be added to the id token instead of requiring the client to use the userinfo endpoint.
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;

        /// <summary>
        /// Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        public int IdentityTokenLifetime { get; set; } = 300;

        /// <summary>
        /// Signing algorithm for identity token. If empty, will use the server default signing algorithm.
        /// </summary>
        public ICollection<string> AllowedIdentityTokenSigningAlgorithms { get; set; } =
            new HashSet<string>();

        /// <summary>
        /// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 3600;

        /// <summary>
        /// Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        public int AuthorizationCodeLifetime { get; set; } = 300;

        /// <summary>
        /// Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days
        /// </summary>
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        /// <summary>
        /// Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days
        /// </summary>
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        /// <summary>
        /// Lifetime of a user consent in seconds. Defaults to null (no expiration)
        /// </summary>
        public int? ConsentLifetime { get; set; } = null;

        /// <summary>
        /// ReUse: the refresh token handle will stay the same when refreshing tokens
        /// OneTime: the refresh token handle will be updated when refreshing tokens
        /// </summary>
        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;

        /// <summary>
        /// Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token request.
        /// Defaults to <c>false</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the token should be updated; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = false;

        /// <summary>
        /// Absolute: the refresh token will expire on a fixed point in time (specified by the AbsoluteRefreshTokenLifetime)
        /// Sliding: when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime). The lifetime will not exceed AbsoluteRefreshTokenLifetime.
        /// </summary>
        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;

        /// <summary>
        /// Specifies whether the access token is a reference token or a self contained JWT token (defaults to Jwt).
        /// </summary>
        public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;

        /// <summary>
        /// Gets or sets a value indicating whether the local login is allowed for this client. Defaults to <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if local logins are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// Specifies which external IdPs can be used with this client (if list is empty all IdPs are allowed). Defaults to empty.
        /// </summary>
        public ICollection<string> IdentityProviderRestrictions { get; set; } =
            new HashSet<string>();

        /// <summary>
        /// Gets or sets a value indicating whether JWT access tokens should include an identifier. Defaults to <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> to add an id; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeJwtId { get; set; } = true;

        /// <summary>
        /// Allows settings claims for the client (will be included in the access token).
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        public ICollection<ClientClaim> Claims { get; set; } = new HashSet<ClientClaim>();

        /// <summary>
        /// Gets or sets a value indicating whether client claims should be always included in the access tokens - or only for client credentials flow.
        /// Defaults to <c>false</c>
        /// </summary>
        /// <value>
        /// <c>true</c> if claims should always be sent; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysSendClientClaims { get; set; }

        /// <summary>
        /// Gets or sets a value to prefix it on client claim types. Defaults to <c>client_</c>.
        /// </summary>
        /// <value>
        /// Any non empty string if claims should be prefixed with the value; otherwise, <c>null</c>.
        /// </value>
        public string? ClientClaimsPrefix { get; set; } = "_client";

        /// <summary>
        /// Gets or sets a salt value used in pair-wise subjectId generation for users of this client.
        /// </summary>
        public string? PairWiseSubjectSalt { get; set; }

        /// <summary>
        /// The maximum duration (in seconds) since the last time the user authenticated.
        /// </summary>
        public int? UserSsoLifetime { get; set; }

        /// <summary>
        /// Gets or sets the type of the device flow user code.
        /// </summary>
        /// <value>
        /// The type of the device flow user code.
        /// </value>
        public string? UserCodeType { get; set; }

        /// <summary>
        /// Gets or sets the device code lifetime.
        /// </summary>
        /// <value>
        /// The device code lifetime.
        /// </value>
        public int DeviceCodeLifetime { get; set; }

        /// <summary>
        /// Gets or sets the allowed CORS origins for JavaScript clients.
        /// </summary>
        /// <value>
        /// The allowed CORS origins.
        /// </value>
        public ICollection<string> AllowedCorsOrigins { get; set; } = new HashSet<string>();

        public ICollection<Guid> Environments { get; set; }

        public ResourceVersion Version { get; set; }

        public IDictionary<string, string>? Properties { get; set; }
            = new Dictionary<string, string>();

        public ICollection<DataConnectorOptions>? DataConnectors { get; set; }

        public ICollection<EnabledProvider>? EnabledProviders { get; set; }

        public string Title => Name;

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }

        public ICollection<Guid> GetEnvironmentIds() => Environments;

        public ClientDependencies ClientDependencies { get; set; }

        public IpAddressFilter IpAddressFilter { get; set; } = new();
    }

    /// <summary>
    /// Access token types.
    /// </summary>
    public enum AccessTokenType
    {
        /// <summary>
        /// Self-contained Json Web Token
        /// </summary>
        Jwt = 0,

        /// <summary>
        /// Reference token
        /// </summary>
        Reference = 1
    }

    /// <summary>
    /// Token expiration types.
    /// </summary>
    public enum TokenExpiration
    {
        /// <summary>
        /// Sliding token expiration
        /// </summary>
        Sliding = 0,

        /// <summary>
        /// Absolute token expiration
        /// </summary>
        Absolute = 1
    }

    /// <summary>
    /// Token usage types.
    /// </summary>
    public enum TokenUsage
    {
        /// <summary>
        /// Re-use the refresh token handle
        /// </summary>
        ReUse = 0,

        /// <summary>
        /// Issue a new refresh token handle every time
        /// </summary>
        OneTimeOnly = 1
    }

    public class ClientClaim
    {
        /// <summary>
        /// The claim type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The claim value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The claim value type
        /// </summary>
        public string ValueType { get; set; } = ClaimValueTypes.String;
    }

    public class DataConnectorOptions
    {
        public string? Name { get; set; }

        public bool Enabled { get; set; }

        public IEnumerable<ConnectorProfileType>? ProfileTypeFilter { get; set; }

        public IEnumerable<DataConnectorProperty>? Properties { get; set; }
    }

    public enum ConnectorProfileType
    {
        UserInfo,
        AccessToken,
        IdentityToken
    }

    public class DataConnectorProperty
    {
        public string? Name { get; set; }

        public string? Value { get; set; }
    }


    public class EnabledProvider
    {
        public string Name { get; set; }

        public bool RequestMfa { get; set; }

        public string? UserIdClaimType { get; set; }
    }

    public class ClientDependencies
    {
        public IEnumerable<ApiScope>? ApiScopes { get; set; } = new HashSet<ApiScope>();

        public IEnumerable<IdentityResource>? IdentityResources { get; set; } =
            new HashSet<IdentityResource>();

        public IEnumerable<ApiResource>? ApiResources { get; set; } = new HashSet<ApiResource>();
    }
}
