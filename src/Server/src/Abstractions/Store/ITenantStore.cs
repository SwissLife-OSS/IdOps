using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface ITenantStore
    {
        Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Tenant>> GetManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
        Task<Tenant> SaveAsync(Tenant tenant, CancellationToken cancellationToken);
    }
}
