using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Query)]
    public class IdentityServerQueries
    {
        public async Task<IEnumerable<Model.IdentityServer>> GetIdentityServersAsync(
            [Service] IIdentityServerService identityServerService,
            CancellationToken cancellationToken)
        {
            return await identityServerService.GetAllAsync(cancellationToken);
        }

        public async Task<IEnumerable<IdentityServerGroup>> GetIdentityServersGroupsAsync(
            [Service] IIdentityServerGroupService identityServerGroupService,
            CancellationToken cancellationToken)
        {
            return await identityServerGroupService.GetGroupsByUserTenants(cancellationToken);
        }

        public async Task<IdentityServerGroup?> GetIdentityServerGroupByTenantAsync(
            string tenant,
            [Service] IIdentityServerGroupService identityServerGroupService,
            CancellationToken cancellationToken)
        {
            return await identityServerGroupService.GetGroupByTenantAsync(tenant,
                cancellationToken);
        }

        public async Task<Model.IdentityServer> GetIdentityServerAsync(
            Guid id,
            [Service] IIdentityServerService identityServerService,
            CancellationToken cancellationToken)
        {
            return await identityServerService.GetByIdAsync(id, cancellationToken);
        }
    }
}
