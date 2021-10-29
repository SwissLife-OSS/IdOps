using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class ApiScopeServiceCollectionExtensions
    {
        public static IServiceCollection AddApiScopes(this IServiceCollection services)
        {
            services.AddSingleton<ApiScopeMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<ApiScopeMessageConsumer>());
            return services;
        }
    }
}
