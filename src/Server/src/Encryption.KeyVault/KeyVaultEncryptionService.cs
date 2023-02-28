using System.Text;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdOps;
using IdOps.Encryption;
using EncryptionAlgorithm = Azure.Security.KeyVault.Keys.Cryptography.EncryptionAlgorithm;


public class KeyVaultEncryptionService : IEncryptionService
{
    private readonly ICryptographyClientProvider _cryptographyClientProvider;

    public KeyVaultEncryptionService(ICryptographyClientProvider cryptographyClient)
    {
        _cryptographyClientProvider = cryptographyClient;
        EncryptionAlgorithm = EncryptionAlgorithm.RsaOaep;
    }

    private EncryptionAlgorithm EncryptionAlgorithm { get; }

    public async Task<string> GetEncryptionKeyNameBase64Async()
    {
        var cryptographyClient = await _cryptographyClientProvider.GetCryptographyClientAsync();
        var nameAsArray = Encoding.UTF8.GetBytes(cryptographyClient.KeyId);
        return Convert.ToBase64String(nameAsArray);
    }

    public async Task<string> EncryptAsync(string input, CancellationToken cancellationToken)
    {
        var inputAsArray = Encoding.UTF8.GetBytes(input);
        var cryptographyClient = await _cryptographyClientProvider.GetCryptographyClientAsync();

        EncryptResult result = await cryptographyClient
            .EncryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Convert.ToBase64String(result.Ciphertext);
    }

    public async Task<string> DecryptAsync(string input, CancellationToken cancellationToken)
    {
        var inputAsArray = Convert.FromBase64String(input);
        var cryptographyClient = await _cryptographyClientProvider.GetCryptographyClientAsync();

        DecryptResult result = await cryptographyClient
            .DecryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Encoding.Default.GetString(result.Plaintext);
    }
}
