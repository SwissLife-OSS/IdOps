using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;
using EncryptionAlgorithm = Azure.Security.KeyVault.Keys.Cryptography.EncryptionAlgorithm;

namespace IdOps.Server.Encryption.KeyVault;

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

    public async Task<EncryptedValue> EncryptAsync(string input, CancellationToken cancellationToken)
    {
        var inputAsArray = Encoding.UTF8.GetBytes(input);
        var cryptographyClient = await _cryptographyClientProvider.GetCryptographyClientAsync();

        EncryptResult result = await cryptographyClient
            .EncryptAsync(EncryptionAlgorithm, inputAsArray, cancellationToken);

        return new KeyVaultEncryptedValue(result);
    }

    public async Task<string> DecryptAsync(EncryptedValue value, CancellationToken cancellationToken)
    {
        if (value is not KeyVaultEncryptedValue keyVaultValue)
        {
            throw new ArgumentException(
                $"The value must be of type {nameof(KeyVaultEncryptedValue)}.",
                nameof(value));
        }

        var cryptographyClient = await _cryptographyClientProvider.GetCryptographyClientAsync();

        DecryptResult result = await cryptographyClient
            .DecryptAsync(EncryptionAlgorithm, keyVaultValue.CipherText, cancellationToken);

        return Encoding.Default.GetString(result.Plaintext);
    }
}
