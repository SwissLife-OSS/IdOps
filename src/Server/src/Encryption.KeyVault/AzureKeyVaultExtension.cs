using IdOps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class AzureKeyVaultExtension
{
    public static IIdOpsServerBuilder AddAzureKeyVault(this IIdOpsServerBuilder builder)
    {
        AzureKeyVaultOptions options =
            builder.Configuration.GetSection("Azure").Get<AzureKeyVaultOptions>();

        builder.Services.AddSingleton<IEncryptionService>(_ => new KeyVaultController(options));
        return builder;
    }
}
