using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Query)]
    public class ApiResourceQueries
    {
        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<IEnumerable<ApiResource>> GetApiResourcesAsync(
            [Service] IApiResourceService apiResourceService,
            GetApiResourcesInput? input,
            CancellationToken cancellationToken)
        {
            return await apiResourceService.GetByTenantsAsync(
                null,
                input?.Tenants ?? Array.Empty<string>(),
                cancellationToken);
        }
    }
}
