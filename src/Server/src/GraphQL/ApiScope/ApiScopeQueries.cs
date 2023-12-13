using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
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
