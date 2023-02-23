using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdOps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class AzureKeyVaultExtension
{
    public static IIdOpsServerBuilder AddAzureKeyVault(this IIdOpsServerBuilder builder)
    {
        AzureKeyVaultOptions options =
            builder.Configuration.GetSection("Azure").Get<AzureKeyVaultOptions>();



        builder.Services.AddSingleton<CryptographyClient>(GetCryptographyClient(options).Result);
        builder.Services.AddSingleton<IEncryptionService,KeyVaultController>();


        return builder;
    }

    private static async Task<CryptographyClient> GetCryptographyClient(AzureKeyVaultOptions options)
    {
        var client = new KeyClient(new Uri(options.KeyVaultUri), new DefaultAzureCredential());
        KeyVaultKey key = await client.GetKeyAsync(options.EncryptionKeyName).ConfigureAwait(false);
        return new CryptographyClient(key.Id, new DefaultAzureCredential());
    }
}
