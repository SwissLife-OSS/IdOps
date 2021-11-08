using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class TenantQueries
    {
        public async Task<IEnumerable<Tenant>> GetTenantsAsync(
            [Service] ITenantService tenantService,
            [Service] IEnumerable<IResourceManager> rm,
            CancellationToken cancellationToken)
        {
            IEnumerable<Tenant> tenants = await tenantService.GetAllAsync(cancellationToken);
            return tenants;
        }
    }
}
