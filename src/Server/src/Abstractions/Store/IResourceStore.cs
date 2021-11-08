using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Server.Storage
{
    public interface IResourceStore
    {
        string Type { get; }

        Task<IReadOnlyList<IResource>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<IResource>> GetAllAsync(
            CancellationToken cancellationToken) =>
            GetAllAsync(null, null, cancellationToken);

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

        new Task<IReadOnlyList<T>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        new Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken) =>
            GetAllAsync(null, null, cancellationToken);

        new Task<IReadOnlyList<T>> GetByIdsAsync(
            IEnumerable<Guid>? ids,
            CancellationToken cancellationToken);
    }
}
