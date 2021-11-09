using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IIdentityServerGroupService
    {
        Task<IReadOnlyList<IdentityServerGroup>> GetAllGroupsAsync(CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityServerGroup?> GetGroupByTenantAsync(string tenant, CancellationToken cancellationToken);
        Task<IEnumerable<IdentityServerGroup>> GetGroupsByUserTenants(CancellationToken cancellationToken);
        Task<IdentityServerGroup> SaveAsync(SaveIdentityServerGroupRequest request, CancellationToken cancellationToken);
    }
}
