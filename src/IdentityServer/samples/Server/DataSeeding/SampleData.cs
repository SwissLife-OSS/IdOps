using System;
using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Samples.DataSeeding
{
    public class SampleData
    {
        private static readonly ICollection<Secret> DefaultSecrets = new List<Secret>
                        { new Secret("DontTell".ToSha256()) };

        private static PublishSource Source => new PublishSource
        {
            Version = 0,
            Id = Guid.Empty,
            ModifiedAt = DateTime.Now
        };

        public static IEnumerable<IdOpsIdentityResource> IdentityResources =>
            new List<IdOpsIdentityResource>
            {
                new IdOpsIdentityResource
                {
                    Name = "openid",
                    ShowInDiscoveryDocument = true,
                    UserClaims = new List<string> { "sub" },
                    Source = Source
                },
                new IdOpsIdentityResource
                {
                    Name = "profile",
                    ShowInDiscoveryDocument = true,
                    UserClaims = new List<string> { "email", "given_name", "family_name" },
                    Source = Source
                }
            };

        public static IEnumerable<IdOpsApiResource> ApiResources =>
            new List<IdOpsApiResource>
            {
                new IdOpsApiResource
                {
                    Name = "api.contacts",
                    DisplayName = "",
                    Scopes = new List<string>
                    {
                        "api.contacts.read",
                        "api.contacts.write",
                    },
                    Source = Source
                }
            };

        public static IEnumerable<IdOpsApiScope> ApiScopes =>
            new List<IdOpsApiScope>
            {
                new IdOpsApiScope
                {
                    Name = "api.contacts.read",
                    DisplayName = "Read Contracts",
                    Source = Source
                },
              new IdOpsApiScope
                {
                    Name = "api.contacts.write",
                    DisplayName = "Write Contracts",
                    Source = Source
                },
            };

        public static IEnumerable<IdOpsClient> Clients =>
            new List<IdOpsClient>
            {
                new IdOpsClient
                {
                    ClientName = "Contracts UI",
                    ClientId = "Contracts.UI",
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = DefaultSecrets,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        "http://localhost:5000/signin-oidc"
                    },
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api.contracts.read",
                    },
                    Source = Source
                },
                new IdOpsClient
                {
                    ClientName = "Test",
                    ClientId = "IdOps.Test",
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = DefaultSecrets,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {
                        "api.contacts.read",
                        "api.contacts.write",
                    },
                    Source = Source
                },
            };
    }
}
