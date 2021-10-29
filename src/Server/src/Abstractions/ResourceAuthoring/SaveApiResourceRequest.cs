using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public record SaveApiResourceRequest(
        string Name,
        string? DisplayName,
        string Tenant) : ResourceSaveRequest, ITenantInput
    {
        public IReadOnlyList<Guid> Scopes { get; init; } = Array.Empty<Guid>();
    }

    public record SaveApiScopeRequest(
        string Name,
        string DisplayName,
        string Tenant) : ResourceSaveRequest, ITenantInput
    {
        public bool ShowInDiscoveryDocument { get; init; } = true;
    }

    public record SaveUserClaimRuleRequest(
        string Name,
        string Tenant) : ITenantInput
    {
        public Guid? Id { get; init; }
        public Guid? ApplicationId { get; init; }
        public IEnumerable<UserClaim> Claims { get; init; } = Array.Empty<UserClaim>();
        public IEnumerable<ClaimRuleMatch> Rules { get; init; } = Array.Empty<ClaimRuleMatch>();
    }

    public record SaveGrantTypeRequest(
        string Id,
        string Name,
        IEnumerable<string> Tenants,
        bool IsCustom)
    {
    }

    public record SaveIdentityResourceRequest(
        string Name,
        string DisplayName,
        Guid IdentityServerGroupId,
        IEnumerable<string> Tenants) : ResourceSaveRequest
    {
        public bool ShowInDiscoveryDocument { get; init; } = true;

        public IEnumerable<string> UserClaims { get; init; } = new HashSet<string>();

        public bool Required { get; set; }

        public bool Emphasize { get; set; }
    }

    public record ResourceSaveRequest
    {
        public Guid? Id { get; init; }

        public bool Enabled { get; init; } = true;

        public string? Description { get; init; }
    }

    public record CreateClientRequest(
        string Name,
        string Tenant,
        IReadOnlyList<string> AllowedGrantTypes,
        IReadOnlyList<Guid> Environments,
        IReadOnlyList<Guid> ApiScopes,
        IReadOnlyList<Guid> IdentityScopes) : ITenantInput
    {
        public string? ClientId { get; init; }

        public string? ClientIdGenerator { get; set; }
    }

    public record UpdateApplicationRequest(
        Guid Id,
        string Name,
        IReadOnlyList<string> AllowedGrantTypes,
        IReadOnlyList<Guid> ApiScopes,
        IReadOnlyList<Guid> IdentityScopes,
        IReadOnlyList<string> RedirectUris)
    {
    }

    public record CreateApplicationRequest(
        string Name,
        string Tenant,
        Guid TemplateId,
        IReadOnlyList<string> AllowedGrantTypes,
        IReadOnlyList<Guid> ApiScopes,
        IReadOnlyList<Guid> IdentityScopes,
        IReadOnlyList<Guid> Environments,
        IReadOnlyList<string> RedirectUris) : ITenantInput
    {
    }

    public record RemoveClientRequest(Guid Id, Guid ClientId);


    public record AddClientRequest(Guid Id, Guid ClientId);

    public record SaveTenantRequest(
        string Id,
        string Color,
        string Description)
    {
        public IReadOnlyList<TenantModule>? Modules { get; init; }

        public IReadOnlyList<TenantRoleMapping>? RoleMappings { get; init; }

        public IReadOnlyList<string>? Emails { get; init; }
    }

    public record SaveEnvironmentRequest(string Name)
    {
        public Guid? Id { get; init; }

        public int Order { get; init; }
    }

    public record AddEnvironmentToApplicationRequest(
        IReadOnlyList<Guid> Environments,
        Guid Id)
    {
    }

    public record SaveClientTemplateRequest(
        string Name,
        string Tenant,
        IReadOnlyList<string> AllowedGrantTypes,
        IReadOnlyList<ClientTemplateSecret> Secrets) : ITenantInput
    {
        public Guid? Id { get; init; }

        public string? ClientIdGenerator { get; init; }

        public string? NameTemplate { get; init; }

        public string? UrlTemplate { get; init; }

        public string? SecretGenerator { get; init; }

        public IReadOnlyList<string>? PostLogoutRedirectUris { get; init; }

        public bool RequirePkce { get; init; } = true;

        public bool RequireClientSecret { get; init; } = true;

        public bool AllowAccessTokensViaBrowser { get; init; } = false;

        public bool AllowOfflineAccess { get; init; } = false;

        public IReadOnlyList<DataConnectorOptions>? DataConnectors { get; init; }

        public IReadOnlyList<EnabledProvider>? EnabledProviders { get; init; }

        public IReadOnlyList<Guid>? ApiScopes { get; init; }

        public IReadOnlyList<Guid>? IdentityScopes { get; init; }

        public IReadOnlyList<string>? RedirectUris { get; init; }
    }
}
