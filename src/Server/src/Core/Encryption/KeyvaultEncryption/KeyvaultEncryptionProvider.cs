using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;

public sealed class KeyvaultEncryptionProvider : IGenericEncryptionProvider<KeyvaultEncryptedValue>
{
    public string Kind => nameof(KeyvaultEncryptedValue);
    
    private readonly CryptographyClient _cryptographyClient;
    
    private EncryptionAlgorithm _encryptionAlgorithm;

    public KeyvaultEncryptionProvider(CryptographyClient cryptographyClient)
    {
        _cryptographyClient = cryptographyClient;
        _encryptionAlgorithm = EncryptionAlgorithm.RsaOaep;
    }

    public async Task<KeyvaultEncryptedValue> EncryptAsync(string value, CancellationToken cancellationToken)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        EncryptResult result = await 
            _cryptographyClient.EncryptAsync(_encryptionAlgorithm, bytes, cancellationToken);
        
        return new KeyvaultEncryptedValue(result);
    }

    public async Task<string> DecryptAsync(KeyvaultEncryptedValue value, CancellationToken cancellationToken)
    {
        DecryptResult result =  await _cryptographyClient.DecryptAsync(_encryptionAlgorithm,
            value.EncryptResult.Ciphertext, cancellationToken);
        string plainText = Encoding.UTF8.GetString(result.Plaintext);
        return plainText;
    }
}