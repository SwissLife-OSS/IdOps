using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace IdOps;
public interface ICryptographyClientProvider
{
    Task<CryptographyClient> GetCryptographyClientAsync();
}
