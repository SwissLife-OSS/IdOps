using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class ClientTemplate : IHasTenant
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Tenant { get; set; } = default!;

        public string? ClientIdGenerator { get; set; }

        public string? SecretGenerator { get; set; } = "DEFAULT";

        public string? NameTemplate { get; set; }

        public string? UrlTemplate { get; set; }

        public bool RequireClientSecret { get; set; } = true;

        public bool RequirePkce { get; set; } = true;

        public ICollection<string> AllowedGrantTypes { get; set; } = new HashSet<string>();

        public bool AllowAccessTokensViaBrowser { get; set; } = false;

        public ICollection<string> RedirectUris { get; set; } = new HashSet<string>();

        public ICollection<string> PostLogoutRedirectUris { get; set; } = new HashSet<string>();

        public bool AllowOfflineAccess { get; set; } = false;

        public ICollection<Guid> ApiScopes { get; set; } = new HashSet<Guid>();

        public ICollection<Guid> IdentityScopes { get; set; } = new HashSet<Guid>();

        public ICollection<DataConnectorOptions>? DataConnectors { get; set; }

        public ICollection<EnabledProvider>? EnabledProviders { get; set; }

        public ICollection<ClientTemplateSecret> Secrets { get; set; }
            = new List<ClientTemplateSecret>();
    }
}

