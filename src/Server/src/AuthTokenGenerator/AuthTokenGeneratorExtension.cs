using IdOps.Abstractions;
using IdOps.Certifaction;
using IdOps.Certification;
using IdOps.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps;

public static class AuthTokenGeneratorExtension
{
    public static IServiceCollection AddAuthTokenGenerator(this IServiceCollection services)
    {
        services.AddSingleton<IDefaultShellService, DefaultShellService>();
        services.AddSingleton<ITokenAnalyzer, TokenAnalyzer>();
        services.AddSingleton<ISettingsStore, SettingsStore>();
        services.AddSingleton<IUserSettingsManager, UserSettingsManager>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddSingleton<IAuthTokenStore, UserDataAuthTokenStore>();
        services.AddHttpClient();

        services.AddSingleton<ICertificateManager, CertificateManager>();
        services.AddSingleton<ISymmetricEncryption, SymmetricEncryption>();
        services.AddSingleton<IDataProtector, CertificateDataProtector>();
        services.AddSingleton<IUserDataProtector, KeyRingUserDataProtector>();


        return services;
    }
}
