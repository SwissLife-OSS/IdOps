using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

public interface IEncryptionProvider
{
    public string Kind { get; }

    Task<EncryptedValue> EncryptAsync(string value, CancellationToken cancellationToken);

    Task<string> DecryptAsync(EncryptedValue value, CancellationToken cancellationToken);
}
