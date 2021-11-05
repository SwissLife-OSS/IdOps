using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IIdentityServerGroupStore
    {
        Task<IReadOnlyList<IdentityServerGroup>> GetAllAsync(CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByTenantAsync(string tenant, CancellationToken cancellationToken);
        Task<IdentityServerGroup> SaveAsync(IdentityServerGroup identityServerGroup, CancellationToken cancellationToken);
    }
}
