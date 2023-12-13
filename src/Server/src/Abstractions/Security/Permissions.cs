using System;
using System.Collections.Generic;

namespace IdOps.Security
{
    public static class Permissions
    {
        public static class Tenant
        {
            public static readonly string Manage = "TENANT_MANAGE";
        }

        public static class Environment
        {
            public static readonly string Manage = "ENV_MANAGE";
        }

        public static class IdentityServer
        {
            public static readonly string Manage = "IDSRV_MANAGE";
        }

        public static class Insights
        {
            public static readonly string All = "INSIGHTS_ALL";
        }

        public static class ClientAuthoring
        {
            public static readonly string Approve = "CA_Approval";
            public static readonly string Read = "CA_READ";
            public static readonly string Edit = "CA_EDIT";
            public static readonly string Publish = "CA_PUBLISH";

            public static class GrantTypes
            {
                public static readonly string Manage = "CA_GRANT_TYPE_MANAGE";
            }

            public static class IdentityResource
            {
                public static readonly string Manage = "CA_IDENTITY_RESOURCE_MANAGE";
            }

            public static class PersonalAccessToken
            {
                public static readonly string Read = "CA_PAT_READ";
                public static readonly string Edit = "CA_PAT_EDIT";
                public static readonly string Publish = "CA_PAT_PUBLISH";
            }
        }

        public static readonly Dictionary<string, List<string>> RoleMap;

        static Permissions()
        {
            RoleMap = new(StringComparer.OrdinalIgnoreCase)
            {
                ["IdOps.Admin"] = new List<string>
                {
                    ClientAuthoring.Approve,
                    ClientAuthoring.Read,
                    ClientAuthoring.Edit,
                    ClientAuthoring.Publish,
                    Tenant.Manage,
                    Environment.Manage,
                    IdentityServer.Manage,
                    ClientAuthoring.GrantTypes.Manage,
                    ClientAuthoring.IdentityResource.Manage,
                    Insights.All,
                    ClientAuthoring.PersonalAccessToken.Read,
                    ClientAuthoring.PersonalAccessToken.Edit,
                    ClientAuthoring.PersonalAccessToken.Publish,
                },
                ["IdOps.Read"] =
                    new List<string>
                    {
                        ClientAuthoring.Read, ClientAuthoring.PersonalAccessToken.Read
                    },
                ["IdOps.Edit"] = new List<string>
                {
                    ClientAuthoring.Read,
                    ClientAuthoring.Edit,
                    ClientAuthoring.Publish,
                    ClientAuthoring.PersonalAccessToken.Read,
                    ClientAuthoring.PersonalAccessToken.Edit,
                    ClientAuthoring.PersonalAccessToken.Publish,
                }
            };
        }
    }
}