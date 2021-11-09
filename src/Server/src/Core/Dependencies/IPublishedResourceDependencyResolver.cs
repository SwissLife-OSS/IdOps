using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IPublishedResourceDependencyResolver
    {
        ValueTask<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            PublishedResource publishedResource,
            CancellationToken cancellationToken);

        ValueTask<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            string resourceType,
            CancellationToken cancellationToken);
    }
}
