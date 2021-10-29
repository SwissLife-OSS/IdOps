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
    public class UserClaimRuleQueries
    {
        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<IEnumerable<UserClaimRule>> GetUserClaimsRulesAsync(
            [Service] IUserClaimRulesService userClaimRulesService,
            GetUserClaimRulesInput input,
            CancellationToken cancellationToken) =>
            await userClaimRulesService.GetByTenantsAsync(null, input.Tenants, cancellationToken);

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<UserClaimRule> GetUserClaimsRuleAsync(
            [Service] IUserClaimRulesService userClaimRulesService,
            Guid id,
            CancellationToken cancellationToken) =>
            await userClaimRulesService.GetByIdAsync(id, cancellationToken);
    }
}
