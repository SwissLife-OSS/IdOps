using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class IdentityServerGroupService : UserTenantService, IIdentityServerGroupService
    {
        private readonly IIdentityServerGroupStore _store;
    
        public IdentityServerGroupService(
            IUserContextAccessor userContextAccessor,
            IIdentityServerGroupStore store)
            :base(userContextAccessor)
        {
            _store = store;
        }
    
        public async Task<IEnumerable<IdentityServerGroup>> GetGroupsByUserTenants(
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> userTenants = await GetUserTenantsAsync(cancellationToken);
            IReadOnlyList<IdentityServerGroup> allGroups = await GetAllGroupsAsync(cancellationToken);
    
            return allGroups.Where(group => group.Tenants.Any(userTenants.Contains));
        }
    
        public Task<IReadOnlyList<IdentityServerGroup>> GetAllGroupsAsync(
            CancellationToken cancellationToken)
        {
            return _store.GetAllAsync(cancellationToken);
        }
    
        public async Task<IdentityServerGroup> SaveAsync(
            SaveIdentityServerGroupRequest request,
            CancellationToken cancellationToken)
        {
            var identityServerGroup = new IdentityServerGroup
            {
                Id = request.Id.GetValueOrDefault(Guid.NewGuid()),
                Name = request.Name,
                Tenants = request.Tenants.ToList(),
                Color = request.Color
            };

            return await _store.SaveAsync(identityServerGroup, cancellationToken);
        }
    
        public async Task<IdentityServerGroup?> GetGroupByTenantAsync(
            string tenant,
            CancellationToken cancellationToken)
        {
            return await _store
                .GetGroupByTenantAsync(tenant, cancellationToken);
        }
    
        public async Task<IdentityServerGroup?> GetGroupByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _store.GetGroupByIdAsync(id, cancellationToken);
        }
    }
}
