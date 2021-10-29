using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddSingleton<ClientMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<ClientMessageConsumer>());
            return services;
        }
    }
}
