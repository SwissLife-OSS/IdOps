using System.Text;
using Azure.Core.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using EncryptionAlgorithm = Azure.Security.KeyVault.Keys.Cryptography.EncryptionAlgorithm;


public class KeyVaultController : IEncryptionService
{
    private readonly CryptographyClient _cryptographyClient;
    public EncryptionAlgorithm EncryptionAlgorithm { get; }

    public KeyVaultController(CryptographyClient cryptographyClient)
    {
        _cryptographyClient = cryptographyClient;
        EncryptionAlgorithm = EncryptionAlgorithm.RsaOaep;
    }

    public string GetEncryptionKeyNameBase64()
    {
        var nameAsArray = Encoding.UTF8.GetBytes(_cryptographyClient.KeyId);
        return Convert.ToBase64String(nameAsArray);
    }

    public async Task<string> EncryptAsync(string input, CancellationToken cancellationToken)
    {
        var inputAsArray = Encoding.UTF8.GetBytes(input);

        EncryptResult result = await _cryptographyClient
            .EncryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Convert.ToBase64String(result.Ciphertext);
    }

    public async Task<string> DecryptAsync(string input, CancellationToken cancellationToken)
    {
        var inputAsArray = Convert.FromBase64String(input);

        DecryptResult result = await _cryptographyClient
            .DecryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Encoding.Default.GetString(result.Plaintext);
    }

}
