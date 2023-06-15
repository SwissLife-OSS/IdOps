using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;
internal class AzureKeyvaultCryptographyClientProvider : ICryptographyClientProvider
{
    private readonly AzureKeyVaultOptions _options;
    private CryptographyClient? _cryptographyClient;

    public AzureKeyvaultCryptographyClientProvider(AzureKeyVaultOptions options)
    {
        _options = options;
    }

    public async Task<CryptographyClient> GetCryptographyClientAsync()
    {
        if (_cryptographyClient == null)
        {
            _cryptographyClient = await CreateCryptographyClient();
        }

        return _cryptographyClient;
    }

    private async Task<CryptographyClient> CreateCryptographyClient()
    {
        var client = new KeyClient(new Uri(_options.KeyVaultUri), new DefaultAzureCredential());
        KeyVaultKey key =
            await client.GetKeyAsync(_options.EncryptionKeyName).ConfigureAwait(false);
        return new CryptographyClient(key.Id, new DefaultAzureCredential());
    }
}
