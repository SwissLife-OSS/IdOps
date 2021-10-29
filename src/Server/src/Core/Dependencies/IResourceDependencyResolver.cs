using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IResourceDependencyResolver
    {
        string ResourceType { get; }

        Task<IReadOnlyList<IResource>> ResolveDependenciesAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
