using System;
using System.Security.Cryptography;
using System.Text;

namespace IdOps
{
    public class GuidClientIdGenerator : IClientIdGenerator
    {
        public string Name => "GUID";

        public string CreateClientId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    public class DefaultSharedSecretGenerator : ISharedSecretGenerator
    {
        public string Name => "DEFAULT";

        public string CreateSecret()
        {
            using RandomNumberGenerator rng = new RNGCryptoServiceProvider();

            byte[] tokenData = new byte[64];
            rng.GetBytes(tokenData);

            var readable = Encoding.UTF8.GetString(tokenData);

            return Convert.ToBase64String(tokenData)
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");
        }
    }
}
