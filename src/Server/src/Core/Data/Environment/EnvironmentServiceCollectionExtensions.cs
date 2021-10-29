using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class EnvironmentServiceCollectionExtensions
    {
        public static IServiceCollection AddEnvironments(this IServiceCollection services)
        {
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            return services;
        }
    }
}
