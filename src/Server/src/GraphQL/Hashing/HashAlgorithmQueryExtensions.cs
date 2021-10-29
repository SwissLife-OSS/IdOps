using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using IdOps.IdentityServer.Hashing;

namespace IdOps.GraphQL.Hashing
{
    [ExtendObjectType(RootTypes.Query)]
    public class HashAlgorithmQueryExtensions
    {
        public IEnumerable<HashAlgorithm> GetHashAlgorithms(
            [Service] IEnumerable<IHashAlgorithm> algorithms) =>
            algorithms.Select(x => new HashAlgorithm(x.AlgorithmType));
    }
}
