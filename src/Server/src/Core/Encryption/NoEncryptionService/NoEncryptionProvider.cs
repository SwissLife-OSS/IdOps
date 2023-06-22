using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

internal class NoEncryptionProvider : IGenericEncryptionProvider<EncryptedValue>
{
    public string Kind => nameof(NoEncryptionProvider);

    public Task<EncryptedValue> EncryptAsync(string value, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<string> DecryptAsync(EncryptedValue value, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
