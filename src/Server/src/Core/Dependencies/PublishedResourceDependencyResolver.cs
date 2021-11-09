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

        public ValueTask<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            PublishedResource publishedResource,
            CancellationToken cancellationToken) =>
            ResolveDependenciesAsync(publishedResource.Id, publishedResource.Type, cancellationToken);

        public async ValueTask<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            string resourceType,
            CancellationToken cancellationToken)
        {
            if (_lookup.TryGetValue(resourceType, out IResourceDependencyResolver? resolver))
            {
                return await resolver.ResolveDependenciesAsync(id, cancellationToken);
            }

            return Array.Empty<IResource>();
        }
    }
}
