using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServers(this IServiceCollection services)
        {
            services.AddSingleton<IIdentityServerService, IdentityServerService>();
            return services;
        }
    }
}
