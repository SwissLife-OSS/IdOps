using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps.Encryption;

public interface ICryptographyClientProvider
{
    Task<CryptographyClient> GetCryptographyClientAsync();
}
