using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps;

internal static class AzureKeyVaultExtension
{
    internal static IServiceCollection AddAzureKeyVault(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AzureKeyVaultOptions options = configuration
            .GetSection("IdOps:Azure:SecretEncryption:AzureKeyVault")
            .Get<AzureKeyVaultOptions>();

        //injects null if no KeyVault options configured but KeyVault is registered on startup
        if (string.IsNullOrEmpty(options.KeyVaultUri) || string.IsNullOrEmpty(options.EncryptionKeyName))
        {
            services.TryAddSingleton<CryptographyClient>(provider => null);
            return services;
        }
        
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
