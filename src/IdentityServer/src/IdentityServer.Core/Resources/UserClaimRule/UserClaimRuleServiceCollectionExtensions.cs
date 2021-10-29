using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class UserClaimRuleServiceCollectionExtensions
    {
        public static IServiceCollection AddUserClaimRules(this IServiceCollection services)
        {
            services.AddSingleton<UserClaimRuleMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<UserClaimRuleMessageConsumer>());
            return services;
        }
    }
}
