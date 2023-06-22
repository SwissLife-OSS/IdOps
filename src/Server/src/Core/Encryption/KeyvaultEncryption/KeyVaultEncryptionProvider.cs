using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;

internal sealed class KeyVaultEncryptionProvider : IGenericEncryptionProvider<KeyVaultEncryptedValue>
{
    public string Kind => nameof(KeyVaultEncryptedValue);

    private readonly CryptographyClient _cryptographyClient;

    private EncryptionAlgorithm _encryptionAlgorithm;

    public KeyVaultEncryptionProvider(CryptographyClient cryptographyClient)
    {
        _cryptographyClient = cryptographyClient;
        _encryptionAlgorithm = EncryptionAlgorithm.RsaOaep;
    }

    public async Task<KeyVaultEncryptedValue> EncryptAsync(string value,
        CancellationToken cancellationToken)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        EncryptResult result =
            await _cryptographyClient.EncryptAsync(_encryptionAlgorithm, bytes, cancellationToken);

        return new KeyVaultEncryptedValue(result);
    }

    public async Task<string> DecryptAsync(KeyVaultEncryptedValue value,
        CancellationToken cancellationToken)
    {
        DecryptResult result = await _cryptographyClient.DecryptAsync(_encryptionAlgorithm,
            value.CipherText, cancellationToken);
        string plainText = Encoding.UTF8.GetString(result.Plaintext);
        return plainText;
    }
}
