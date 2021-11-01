using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Server.Storage
{
    public interface ITenantResourceStore<T> : IResourceStore<T>
        where T : class, ITenantResource, new()
    {
        Task<IReadOnlyList<T>> SearchByTenantAsync(
            string tenant,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<T>> GetByTenantsAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);
    }
}
