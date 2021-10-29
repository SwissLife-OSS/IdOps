using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// A registry of <see cref="IHashAlgorithm"/>. Can resolve a encryptor of a given kind
    /// </summary>
    public class HashAlgorithmResolver
        : IHashAlgorithmResolver
    {
        private readonly IReadOnlyDictionary<string, IHashAlgorithm> _algorithms;

        /// <summary>
        /// Creates a new instance of the <see cref="HashAlgorithmResolver"/>
        /// </summary>
        /// <param name="algorithms"></param>
        public HashAlgorithmResolver(IEnumerable<IHashAlgorithm> algorithms)
        {
            _algorithms = algorithms?.ToDictionary(x => x.AlgorithmType) ??
                throw new ArgumentNullException(nameof(algorithms));
        }

        /// <inheritdoc />
        public bool TryResolve(
            string algorithmType,
            [NotNullWhen(true)] out IHashAlgorithm? encryptor)
        {
            return _algorithms.TryGetValue(algorithmType, out encryptor);
        }
    }
}
