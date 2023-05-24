using System.Threading;
using System.Threading.Tasks;

namespace IdOps;

// TODO: Review the interface, it's to KV specific
public interface IEncryptionService
{
    Task<string> GetEncryptionKeyNameBase64Async();

    Task<string> EncryptAsync(string input, CancellationToken cancellationToken);

    Task<string> DecryptAsync(string input, CancellationToken cancellationToken);
}
