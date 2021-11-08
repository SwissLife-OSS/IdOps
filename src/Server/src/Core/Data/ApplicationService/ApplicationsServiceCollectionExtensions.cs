using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class ApplicationsServiceCollectionExtensions
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            return services
                .AddSingleton<IApplicationService, ApplicationService>()
                .AddSingleton<IResourceDependencyResolver, ApplicationDependencyResolver>();
        }
    }
}
