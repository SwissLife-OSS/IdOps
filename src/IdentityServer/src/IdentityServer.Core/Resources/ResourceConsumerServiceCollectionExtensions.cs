using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class ResourceConsumerServiceCollectionExtensions
    {
        public static IServiceCollection AddResources(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceConsumer, ResourceConsumer>();

            services.AddApiResources();
            services.AddApiScopes();
            services.AddClients();
            services.AddIdentityResources();
            services.AddPersonalAccessTokens();
            services.AddUserClaimRules();

            return services;
        }
    }
}
