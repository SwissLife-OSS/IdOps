using IdOps.Controller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps;

public static class AzureKeyVaultExtension
{
    public static IIdOpsServerBuilder AddAzureKeyVault(this IIdOpsServerBuilder builder)
    {
        AzureKeyVaultOptions options =
            builder.Configuration.GetSection("Azure").Get<AzureKeyVaultOptions>();

        builder.Services.AddSingleton<IKeyVaultController>(_ => new KeyVaultController(options));
        return builder;
    }
}
