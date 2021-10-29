using System;

namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// A password hasher and validator of type PBKDF2
    /// </summary>
    public sealed class Pbkdf2HashAlgorithm : IHashAlgorithm
    {
        public static string Identifier =>  "PBKDF2";

        /// <inheritdoc />
        public string AlgorithmType => Identifier;

        /// <inheritdoc />
        public bool Verify(string secret, string password)
        {
            if (secret == null)
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Pbkdf2Algorithm.ValidatePasswordHash(password, secret);
        }

        /// <inheritdoc />
        public string ComputeHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return Pbkdf2Algorithm.CreatePasswordHash(password);
        }
    }
}
