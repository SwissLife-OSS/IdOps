using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public abstract class ResourceDependencyResolver<T> : IResourceDependencyResolver
        where T : class, IResource, new()
    {
        private static readonly string _resourceType = typeof(T).Name;

        public string ResourceType => _resourceType;

        public abstract Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
