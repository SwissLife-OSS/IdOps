using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class GrantTypesServiceCollectionExtensions
    {
        public static IServiceCollection AddGrantTypes(this IServiceCollection services)
        {
            services.AddSingleton<IGrantTypeService, GrantTypeService>();

            return services;
        }
    }
}
