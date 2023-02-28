using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdOps;
using IdOps.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class AzureKeyVaultExtension
{
    public static IIdOpsServerBuilder AddAzureKeyVault(this IIdOpsServerBuilder builder)
    {
        AzureKeyVaultOptions options =
            builder.Configuration.GetSection("Azure").Get<AzureKeyVaultOptions>();

        builder.Services.AddSingleton<ICryptographyClientProvider>(
            new CryptographyClientProvider(options));
        builder.Services.AddSingleton<IEncryptionService, KeyVaultEncryptionService>();

        return builder;
    }
}
