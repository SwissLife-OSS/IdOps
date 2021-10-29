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
    public class ApiScopeQueries
    {
        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<IEnumerable<ApiScope>> GetApiScopesAsync(
            [Service] IApiScopeService apiScopeService,
            GetApiScopesInput? input,
            CancellationToken cancellationToken)
        {
            return await apiScopeService.GetByTenantsAsync(
                null,
                input?.Tenants ?? Array.Empty<string>(),
                cancellationToken);
        }
    }
}
