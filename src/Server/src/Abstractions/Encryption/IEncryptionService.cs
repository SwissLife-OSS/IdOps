using System.Threading;
using System.Threading.Tasks;

public interface IEncryptionService
{
    Task<string> GetEncryptionKeyNameBase64Async();

    Task<string> EncryptAsync(string input, CancellationToken cancellationToken);

    Task<string> DecryptAsync(string input, CancellationToken cancellationToken);
}
