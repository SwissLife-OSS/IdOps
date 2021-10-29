using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthorization(o =>
            {
                o.AddPolicy(
                    AuthorizationPolicies.Names.ApiAccess,
                    AuthorizationPolicies.ApiAccessPolicy);

                o.AddPolicy(
                    AuthorizationPolicies.Names.ResourceAuthoringRead,
                    AuthorizationPolicies.ResourceAuthoringRead);

                o.AddPolicy(
                    AuthorizationPolicies.Names.ResourceAuthoringEdit,
                    AuthorizationPolicies.ResourceAuthoringEdit);

                o.AddPolicy(
                    AuthorizationPolicies.Names.ResourceAuthoringPublish,
                    AuthorizationPolicies.ResourceAuthoringPublish);

                o.AddPolicy(
                    AuthorizationPolicies.Names.TenantResourceAccess,
                    AuthorizationPolicies.TenantResourceAccess);

                o.AddPolicy(
                    AuthorizationPolicies.Names.TenantManage,
                    AuthorizationPolicies.TenantManage);

                o.AddPolicy(
                    AuthorizationPolicies.Names.IdentityServerManage,
                    AuthorizationPolicies.IdentityServerManage);

                o.AddPolicy(
                    AuthorizationPolicies.Names.ResourceApproval,
                    AuthorizationPolicies.ResourceApproval);

                o.AddPolicy(
                    AuthorizationPolicies.Names.PersonalAccessTokenRead,
                    AuthorizationPolicies.PersonalAccessTokenRead);

                o.AddPolicy(
                    AuthorizationPolicies.Names.PersonalAccessTokenEdit,
                    AuthorizationPolicies.PersonalAccessTokenEdit);

                o.AddPolicy(
                    AuthorizationPolicies.Names.PersonalAccessTokenPublish,
                    AuthorizationPolicies.PersonalAccessTokenPublish);
            });

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, TenantResourceAuthorizationHandler>();

            return services;
        }
    }
}
