using IdOps.Security;
using Microsoft.AspNetCore.Authorization;

namespace IdOps.Authorization
{
    public static class AuthorizationPolicies
    {
        public static class Names
        {
            public const string ApiAccess = nameof(ApiAccessPolicy);
            public const string ResourceAuthoringRead = nameof(ResourceAuthoringRead);
            public const string ResourceApproval = nameof(ResourceApproval);
            public const string ResourceAuthoringEdit = nameof(ResourceAuthoringEdit);
            public const string ResourceAuthoringPublish = nameof(ResourceAuthoringPublish);
            public const string TenantResourceAccess = nameof(TenantResourceAccess);
            public const string TenantManage = nameof(TenantManage);
            public const string IdentityServerManage = nameof(IdentityServerManage);
            public const string PersonalAccessTokenRead = nameof(PersonalAccessTokenRead);
            public const string PersonalAccessTokenEdit = nameof(PersonalAccessTokenEdit);
            public const string PersonalAccessTokenPublish = nameof(PersonalAccessTokenPublish);
        }

        public static AuthorizationPolicy ApiAccessPolicy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            }
        }

        public static AuthorizationPolicy ResourceAuthoringRead
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.Read)
                    .Build();
            }
        }

        public static AuthorizationPolicy ResourceAuthoringEdit
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.Edit)
                    .Build();
            }
        }

        public static AuthorizationPolicy ResourceAuthoringPublish
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.Publish)
                    .Build();
            }
        }

        public static AuthorizationPolicy TenantResourceAccess
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new HasTenantRequirement())
                    .Build();
            }
        }


        public static AuthorizationPolicy TenantManage
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.Tenant.Manage)
                    .Build();
            }
        }


        public static AuthorizationPolicy IdentityServerManage
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.IdentityServer.Manage)
                    .Build();
            }
        }

        public static AuthorizationPolicy ResourceApproval
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.Approve)
                    .Build();
            }
        }

        public static AuthorizationPolicy PersonalAccessTokenRead
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.PersonalAccessToken.Read)
                    .Build();
            }
        }

        public static AuthorizationPolicy PersonalAccessTokenEdit
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.PersonalAccessToken.Edit)
                    .Build();
            }
        }

        public static AuthorizationPolicy PersonalAccessTokenPublish
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .RequirePermission(Permissions.ClientAuthoring.PersonalAccessToken.Publish)
                    .Build();
            }
        }
    }
}
