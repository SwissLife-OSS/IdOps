using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

public interface IGenericEncryptionProvider<T> : IEncryptionProvider where T : EncryptedValue
{
    Task<T> EncryptAsync(string value, CancellationToken cancellationToken);

    Task<string> DecryptAsync(T value, CancellationToken cancellationToken);

    async Task<EncryptedValue> IEncryptionProvider.EncryptAsync(
        string value,
        CancellationToken cancellationToken)
    {
        return await EncryptAsync(value, cancellationToken);
    }

    async Task<string> IEncryptionProvider.DecryptAsync(
        EncryptedValue value,
        CancellationToken cancellationToken)
    {
        if (value is not T encryptedValue)
        {
            throw new ArgumentException(
                $"The value must be of type {typeof(T).Name}.",
                nameof(value));
        }

        return await DecryptAsync(encryptedValue, cancellationToken);
    }
}

