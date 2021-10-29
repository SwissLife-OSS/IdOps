using System.Diagnostics.CodeAnalysis;

namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// Represents a registry of <see cref="IHashAlgorithm"/>. Can resolve a encryptor of a given kind
    /// </summary>
    public interface IHashAlgorithmResolver
    {
        /// <summary>
        /// Try to resolve a encryptor of kind <paramref name="algorithmType"/>
        /// </summary>
        /// <param name="algorithmType">The name of the encryptor</param>
        /// <param name="encryptor">The resolver <see cref="IHashAlgorithm"/></param>
        /// <returns></returns>
        bool TryResolve(
            string algorithmType,
            [NotNullWhen(true)] out IHashAlgorithm? encryptor);
    }
}
