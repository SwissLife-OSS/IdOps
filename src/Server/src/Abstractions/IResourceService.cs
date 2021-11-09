using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IResourceService
    {
        bool RequiresApproval(Guid id);

        string ResourceType { get; }

        // TODO this is temporary
        bool IsAllowedToPublish();

        // TODO this is temporary
        bool IsAllowedToApprove();

        ValueTask<IResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);
    }

    public interface IResourceService<T> : IResourceService
        where T : class, IResource, new()
    {
        new Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);
    }
}
