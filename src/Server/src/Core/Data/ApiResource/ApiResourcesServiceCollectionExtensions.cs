using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class ApiResourcesServiceCollectionExtensions
    {
        public static IServiceCollection AddApiResources(this IServiceCollection services)
        {
            services
                .AddResource<ApiResource>()
                .AddDependencyResolver<ApiResourceDependencyResolver>()
                .AddMessageFactory<ApiResourceMessageFactory>()
                .AddService<IApiResourceService, ApiResourceService>();

            return services;
        }
    }
}
