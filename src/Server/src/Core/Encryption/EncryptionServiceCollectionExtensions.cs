using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IdOps;

public static class EncryptionServiceCollectionExtensions
{
    public static IServiceCollection AddEncryptionProvider<T>(
        this IServiceCollection services,
        bool isDefault = false) where T : class, IEncryptionProvider
    {
        services.AddSingleton<IEncryptionProvider, T>();
        services.TryAddSingleton<IEncryptionService, EncryptionService>();

        services.AddOptions<EncryptionServiceOptions>().Configure<IEnumerable<IEncryptionProvider>>(
            (options, providers) =>
            {
                if (isDefault)
                {
                    options.DefaultProvider = providers.OfType<T>().First();
                }
            });

        services.TryAddSingleton(sp =>
            sp.GetRequiredService<IOptions<EncryptionServiceOptions>>().Value);

        return services;
    }
}
