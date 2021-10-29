using System;
using System.Security.Cryptography;

namespace IdOps.IdentityServer.Hashing
{
    public class PasswordProvider
        : IPasswordProvider,
          IDisposable
    {
        private readonly RandomNumberGenerator _rngCsp = RandomNumberGenerator.Create();

        /// <summary>
        /// Generates a Random password
        /// </summary>
        /// <param name="length">Number of chars</param>
        /// <param name="includeSpecialChars">Include spaecial chars (@-.'$!?*)</param>
        /// <returns>the generated password</returns>
        public string GenerateRandomPassword(int length, bool includeSpecialChars = false)
        {
            var possible = "abcdefghjklmnpqrstuvwyzABCDEFGHJKMNOPQRSTUVWYZ123456789";
            if (includeSpecialChars)
            {
                possible += "@-.'$!?*";
            }

            return GetRandomString(possible, length);
        }

        private string GetRandomString(string possible, int length)
        {
            char[] possibleChars = possible.ToCharArray();
            byte[] randomSequence = new byte[length];
            char[] result = new char[length];

            // Fill Random
            _rngCsp.GetBytes(randomSequence);

            for (int i = 0; i < length; i++)
            {
                result[i] = possibleChars[randomSequence[i] % possibleChars.Length];
            }

            return new string(result);
        }

        public void Dispose()
        {
            _rngCsp.Dispose();
        }
    }
}
