using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class ApplicationsServiceCollectionExtensions
    {
        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationService, ApplicationService>();

            return services;
        }
    }
}
