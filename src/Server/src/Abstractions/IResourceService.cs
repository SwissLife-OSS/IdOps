using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IResourceService
    {
        bool IsOfType(IResource resource);

        bool IsOfType(string resource);

        bool RequiresApproval(Guid id) => false;

        bool ForcePublish => false;

        string ResourceType { get; }

        // TODO this is temporary
        bool IsAllowedToPublish();

        // TODO this is temporary
        bool IsAllowedToApprove();

        ValueTask<IResource?> GetResourceByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);
    }

    public interface IResourceService<T> : IResourceService
        where T : class, IResource, new()
    {
        ValueTask<T?> GetResourceByIdAsync(Guid id, CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);
    }
}
