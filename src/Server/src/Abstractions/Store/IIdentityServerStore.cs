using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IIdentityServerStore : IResourceStore<Model.IdentityServer>
    {
        Task<IEnumerable<Model.IdentityServer>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerGroup>> GetAllGroupsAsync(CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByTenantAsync(string tenant, CancellationToken cancellationToken);
    }
}
