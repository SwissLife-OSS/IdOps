using IdOps.Model;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class PersonalAccessTokensServiceCollectionExtensions
    {
        public static IServiceCollection AddPersonalAccessTokens(this IServiceCollection services)
        {
            services
                .AddResource<PersonalAccessToken>()
                .AddMessageFactory<PersonalAccessTokenMessageFactory>()
                .AddService<IPersonalAccessTokenService, PersonalAccessTokenService>();

            return services;
        }
    }
}
