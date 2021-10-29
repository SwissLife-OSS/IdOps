using System;
using Microsoft.AspNetCore.Identity;
using static Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// A password hasher and validator of type SSHA
    /// </summary>
    public sealed class SshaHashAlgorithm : IHashAlgorithm
    {
        public static string Identifier =>  "SSHA";

        private readonly IPasswordHasher<string> _passwordHasher;

        /// <summary>
        /// Creates a new instance of <see cref="SshaHashAlgorithm"/>
        /// </summary>
        /// <param name="passwordHasher"></param>
        public SshaHashAlgorithm(IPasswordHasher<string> passwordHasher)
        {
            _passwordHasher = passwordHasher ??
                throw new ArgumentNullException(nameof(passwordHasher));
        }

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

            return _passwordHasher
                    .VerifyHashedPassword(Identifier, secret, password) is
                Success or SuccessRehashNeeded;
        }

        /// <inheritdoc />
        public string ComputeHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            return _passwordHasher.HashPassword(Identifier, password);
        }
    }
}
