using IdOps.Abstractions;
using IdOps.Certification;
using IdOps.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps;

public static class AuthTokenGeneratorExtension
{
    public static IServiceCollection AddAuthTokenGenerator(this IServiceCollection services)
    {
        services.AddSingleton<ITokenAnalyzer, TokenAnalyzer>();
        services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddHttpClient();
        
        return services;
    }
}
