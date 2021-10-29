using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public static class PersonalAccessTokenServiceCollectionExtensions
    {
        public static IServiceCollection AddPersonalAccessTokens(this IServiceCollection services)
        {
            services.AddSingleton<PersonalAccessTokenMessageConsumer>();
            services.AddSingleton<IResourceMessageConsumer>(x =>
                x.GetRequiredService<PersonalAccessTokenMessageConsumer>());
            return services;
        }
    }
}
