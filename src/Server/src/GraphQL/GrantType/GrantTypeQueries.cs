using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Query)]
    public class GrantTypeQueries
    {
        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceAuthoringRead)]
        public async Task<IEnumerable<GrantType>> GetGrantTypesAsync(
            [Service] IGrantTypeService grantTypeService,
            CancellationToken cancellationToken)
        {
            return await grantTypeService.GetAllAsync(cancellationToken);
        }
    }
}
