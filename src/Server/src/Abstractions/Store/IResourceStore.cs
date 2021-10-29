using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Store
{
    public interface IResourceStore
    {
        bool IsOfType(IResource resource);

        bool IsOfType(string resource);

        Task<IReadOnlyList<IResource>> GetResourceApprovals(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetAllAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetByIdsAsync(
            IEnumerable<Guid>? ids,
            CancellationToken cancellationToken);

        Task<IResource> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IResource> SaveAsync(IResource resource, CancellationToken cancellationToken);
    }

    public interface IResourceStore<T> : IResourceStore
        where T : class, IResource, new()
    {
        new Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<T> SaveAsync(T resource, CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetResourceWithOpenApproval(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetByIdsAsync(
            IEnumerable<Guid>? ids,
            CancellationToken cancellationToken);
    }
}
