using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public class PublishedResourceDependencyResolver
        : IPublishedResourceDependencyResolver
    {
        private readonly IDictionary<string, IResourceDependencyResolver> _lookup;

        public PublishedResourceDependencyResolver(
            IEnumerable<IResourceDependencyResolver> resolvers)
        {
            _lookup = resolvers.ToDictionary(x => x.ResourceType);
        }

        public async ValueTask<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            PublishedResource publishedResource,
            CancellationToken cancellationToken)
        {
            if (_lookup.TryGetValue(publishedResource.Type, out var resolver))
            {
                return await resolver
                    .ResolveDependenciesAsync(publishedResource.Id, cancellationToken);
            }

            return Array.Empty<IResource>();
        }
    }
}
