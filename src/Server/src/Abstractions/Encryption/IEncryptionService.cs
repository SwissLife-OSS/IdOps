using System.Threading;
using System.Threading.Tasks;

public interface IEncryptionService
{
    string GetEncryptionKeyNameBase64();

    Task<string> EncryptAsync(string input, CancellationToken cancellationToken);

    Task<string> DecryptAsync(string input, CancellationToken cancellationToken);
}
