using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;

namespace IdOps.IdentityServer.Store
{
    public interface IPersistedGrantRepository
    {
        Task DeleteAsync(string key, CancellationToken cancellationToken);
        Task DeleteByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken);

        Task<PersistedGrant?> GetAsync(
            string key, CancellationToken
            cancellationToken);

        Task<IEnumerable<PersistedGrant>> GetByFilterAsync(
            PersistedGrantFilter filter,
            CancellationToken cancellationToken);

        Task<PersistedGrant> SaveAsync(
            PersistedGrant entity,
            CancellationToken cancellationToken);
    }
}
