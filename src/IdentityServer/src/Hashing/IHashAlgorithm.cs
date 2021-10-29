namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// Represents a password hasher and validator
    /// </summary>
    public interface IHashAlgorithm
    {
        /// <summary>
        /// The encryption kind this encryptor can handle
        /// </summary>
        string AlgorithmType { get; }

        /// <summary>
        /// Computes a hash of the <paramref name="password"/>
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns></returns>
        string ComputeHash(string password);

        /// <summary>
        /// Verifies if a given password matches the secret
        /// </summary>
        /// <param name="secret">The hashed password</param>
        /// <param name="password">The password to hash</param>
        /// <returns>True if the secret is the hashed password</returns>
        bool Verify(string secret, string password);
    }
}
