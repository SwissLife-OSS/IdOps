using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

internal class NoEncryptionService : IEncryptionService
{
    public Task<string> GetEncryptionKeyNameBase64Async()
    {
        throw new NotSupportedException();
    }

    public Task<string> EncryptAsync(string input, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<string> DecryptAsync(string input, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
