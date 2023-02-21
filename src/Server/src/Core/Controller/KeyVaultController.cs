using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IdOps.Controller;

public class KeyVaultController : IKeyVaultController
{
    private readonly string _keyVaultUri;
    private readonly string _keyVaultKeyName;

    private readonly CryptographyClient _cryptographyClient;
    public EncryptionAlgorithm EncryptionAlgorithm { get; }

    public KeyVaultController(AzureKeyVaultOptions options)
    {


        _keyVaultUri = options.KeyVaultUri;
        _keyVaultKeyName = options.EncryptionKeyName;
        _cryptographyClient = GetCryptographyClient().Result;
        EncryptionAlgorithm = EncryptionAlgorithm.RsaOaep;
    }

    public string GetEncryptionKeyNameBase64()
    {
        var nameAsArray = Encoding.UTF8.GetBytes(_keyVaultKeyName);
        return Convert.ToBase64String(nameAsArray);
    }

    public async Task<string> Encrypt(string input)
    {
        var inputAsArray = Encoding.UTF8.GetBytes(input);

        EncryptResult result = await _cryptographyClient
            .EncryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Convert.ToBase64String(result.Ciphertext);
    }

    public async Task<string> Decrypt(string input)
    {
        var inputAsArray = Convert.FromBase64String(input);

        DecryptResult result = await _cryptographyClient
            .DecryptAsync(EncryptionAlgorithm, inputAsArray).ConfigureAwait(false);

        return Encoding.Default.GetString(result.Plaintext);
    }

    private async Task<CryptographyClient> GetCryptographyClient()
    {
        var client = new KeyClient(new Uri(_keyVaultUri), new DefaultAzureCredential());
        KeyVaultKey key = await client.GetKeyAsync(_keyVaultKeyName).ConfigureAwait(false);
        return new CryptographyClient(key.Id, new DefaultAzureCredential());
    }
}
