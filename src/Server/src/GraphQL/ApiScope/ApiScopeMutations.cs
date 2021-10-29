using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class ApiScopeMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveApiScopePayload> SaveApiScopeAsync(
            SaveApiScopeRequest input,
            [Service] IApiScopeService apiScopeService,
            CancellationToken cancellationToken)
        {
            ApiScope apiResource = await apiScopeService.SaveAsync(
                input,
                cancellationToken);

            return new SaveApiScopePayload(apiResource);
        }
    }
}
