using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

public interface IEncryptionService
{
    Task<EncryptedValue> EncryptAsync(string value, CancellationToken cancellationToken);

    Task<string> DecryptAsync(EncryptedValue value, CancellationToken cancellationToken);
}
