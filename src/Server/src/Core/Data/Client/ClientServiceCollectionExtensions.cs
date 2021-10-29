using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientIdGenerator, GuidClientIdGenerator>();

            services
                .AddResource<Client>()
                .AddDependencyResolver<ClientDependencyResolver>()
                .AddMessageFactory<ClientMessageFactory>()
                .AddService<IClientService, ClientService>();

            return services;
        }
    }
}
