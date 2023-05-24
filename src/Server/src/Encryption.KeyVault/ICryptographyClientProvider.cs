using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps.Server.Encryption.KeyVault;

public interface ICryptographyClientProvider
{
    Task<CryptographyClient> GetCryptographyClientAsync();
}
