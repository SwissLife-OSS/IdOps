using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]

    public class TenantMutations
    {
        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.TenantManage)]
        public async Task<SaveTenantPayload> SaveTenantAsync(
            SaveTenantRequest input,
            [Service] ITenantService tenantService,
            CancellationToken cancellationToken)
        {
            Tenant tenant = await tenantService.SaveAsync(input, cancellationToken);

            return new SaveTenantPayload(tenant);
        }
    }
}
