using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class IdentityResourceServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityResources(this IServiceCollection services)
        {
            services.AddSingleton<IdentityResourceMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<IdentityResourceMessageConsumer>());
            return services;
        }
    }
}
