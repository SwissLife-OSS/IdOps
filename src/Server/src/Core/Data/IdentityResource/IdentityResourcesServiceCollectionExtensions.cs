using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class IdentityResourcesServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityResources(this IServiceCollection services)
        {
            services
                .AddResource<IdentityResource>()
                .AddMessageFactory<IdentityResourceMessageFactory>()
                .AddService<IIdentityResourceService, IdentityResourceService>();

            return services;
        }
    }
}
