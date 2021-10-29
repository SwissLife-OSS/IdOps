using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface ISecretCryptoProvider
    {
        Task<SecretEncryptionResult> EncryptAsync(
            string value,
            CancellationToken cancellationToken);

        Task<string> DecryptAsync(
            string cipherValue,
            SecretEncryptionInfo encryptionInfo,
            CancellationToken cancellationToken);
    }

    public class SecretEncryptionInfo
    {
        public string KeyProvider { get; set; }

        public string Key { get; set; }

        public string Algorithm { get; set; }
    }

    public record SecretEncryptionResult(
        SecretEncryptionInfo EncryptionInfo,
        string CipherValue);
}
