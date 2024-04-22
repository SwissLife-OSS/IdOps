using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

internal class EncryptionService : IEncryptionService
{
    private readonly EncryptionServiceOptions _options;
    private readonly IReadOnlyDictionary<string, IEncryptionProvider> _providers;

    public EncryptionService(
        IEnumerable<IEncryptionProvider> providers,
        EncryptionServiceOptions options)
    {
        _options = options;
        _providers = providers.ToDictionary(x => x.Kind);
    }

    public async Task<EncryptedValue> EncryptAsync(string value,
        CancellationToken cancellationToken)
    {
        return await _options.DefaultProvider.EncryptAsync(value, cancellationToken);
    }

    public async Task<string> DecryptAsync(EncryptedValue value,
        CancellationToken cancellationToken)
    {
        if (!_providers.TryGetValue(value.Kind, out var provider))
        {
            throw new ArgumentException($"The encryption provider {value.Kind} is not supported.",
                nameof(value));
        }

        return await provider.DecryptAsync(value, cancellationToken);
    }
}
