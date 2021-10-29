using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class GrantTypeMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<SaveGrantTypePayload> SaveGrantTypeAsync(
            SaveGrantTypeRequest input,
            [Service] IGrantTypeService grantTypeService,
            CancellationToken cancellationToken)
        {
            GrantType grantType = await grantTypeService.SaveAsync(
                input,
                cancellationToken);

            return new SaveGrantTypePayload(grantType);
        }
    }
}
