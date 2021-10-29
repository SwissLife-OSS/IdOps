using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class UserClaimRulesServiceCollectionExtensions
    {
        public static IServiceCollection AddUserClaimRules(this IServiceCollection services)
        {
            services
                .AddResource<UserClaimRule>()
                .AddMessageFactory<UserClaimRuleMessageFactory>()
                .AddService<IUserClaimRulesService, UserClaimRulesService>();

            return services;
        }
    }
}
