using IdOps.IdentityServer.Model;
using Microsoft.Extensions.DependencyInjection;


namespace IdOps.IdentityServer.DataConnector
{
    public static class UserDataConnectorServiceCollectionExtensions
    {
        public static IServiceCollection AddUserDataConnectors(
            this IServiceCollection services)
        {
            services.AddScoped<IUserDataConnectorService, UserDataConnectorService>();
            services.AddSingleton<IUserDataConnector, ClaimsRulesDataConnector>();

            return services;
        }
    }
}
