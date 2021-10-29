using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class ApiScopesServiceCollectionExtensions
    {
        public static IServiceCollection AddApiScopes(this IServiceCollection services)
        {
            services
                .AddResource<ApiScope>()
                .AddMessageFactory<ApiScopeMessageFactory>()
                .AddService<IApiScopeService, ApiScopeService>();

            return services;
        }
    }
}
