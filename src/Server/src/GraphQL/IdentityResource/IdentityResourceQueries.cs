using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class IdentityResourceQueries
    {
        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public async Task<IEnumerable<IdentityResource>> GetIdentityResourcesAsync(
            GetIdentityResourcesInput? input,
            [Service] IIdentityResourceService identityResourceService,
            CancellationToken cancellationToken)
        {
            return await identityResourceService.GetByTenantsAsync(
                null,
                input?.Tenants ?? Array.Empty<string>(),
                cancellationToken);
        }
    }
}
