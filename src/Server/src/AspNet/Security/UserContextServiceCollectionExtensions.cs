using IdOps.Security;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.Api.Security
{
    public static class UserContextServiceCollectionExtensions
    {
        public static IServiceCollection AddUserContextAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IUserContextFactory, ClaimsPrincipalUserContextFactory>();
            services.AddSingleton<IUserContextAccessor, UserContextAccessor>();
            return services;
        }
    }
}
