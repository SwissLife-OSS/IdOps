using IdOps;
using IdOps.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.Server.Encryption.KeyVault;

public static class AzureKeyVaultExtension
{
    public static IIdOpsServerBuilder AddAzureKeyVault(this IIdOpsServerBuilder builder)
    {
        AzureKeyVaultOptions options = builder.Configuration
            .GetSection("SecretEncryption:AzureKeyVault")
            .Get<AzureKeyVaultOptions>();

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<ICryptographyClientProvider, CryptographyClientProvider>();
        builder.Services.AddSingleton<IEncryptionService, KeyVaultEncryptionService>();

        return builder;
    }
}
