using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class ApiResourceServiceCollectionExtensions
    {
        public static IServiceCollection AddApiResources(this IServiceCollection services)
        {
            services.AddSingleton<ApiResourceMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<ApiResourceMessageConsumer>());
            return services;
        }
    }
}
