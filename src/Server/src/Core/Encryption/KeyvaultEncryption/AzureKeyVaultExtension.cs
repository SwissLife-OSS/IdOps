using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps;

public static class AzureKeyVaultExtension
{
    public static IServiceCollection AddAzureKeyVault(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AzureKeyVaultOptions options = configuration
            .GetSection("IdOps:Azure:SecretEncryption:AzureKeyVault")
            .Get<AzureKeyVaultOptions>();

        services.AddSingleton(options);
        services.AddSingleton<ICryptographyClientProvider, AzureKeyVaultCryptographyClientProvider>();
        services.TryAddSingleton<CryptographyClient>(provider =>
        {
            var cryptographyClientProvider = provider.GetRequiredService<ICryptographyClientProvider>();
            return cryptographyClientProvider.GetCryptographyClientAsync().GetAwaiter().GetResult();
        });
        return services;
    }
}
