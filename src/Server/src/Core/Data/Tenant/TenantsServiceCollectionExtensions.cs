using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class TenantsServiceCollectionExtensions
    {
        public static IServiceCollection AddTenants(this IServiceCollection services)
        {
            services.AddSingleton<ITenantService, TenantService>();
            services.AddSingleton<ITenantUserRoleResolver, TenantUserRoleResolver>();

            return services;
        }
    }
}
