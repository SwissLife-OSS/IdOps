using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class IdentityResourceMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<SaveIdentityResourcePayload> SaveIdentityResourceAsync(
            [Service] IIdentityResourceService identityResourceService,
            SaveIdentityResourceRequest input,
            CancellationToken cancellationToken)
        {
            IdentityResource identityResource = await identityResourceService.SaveAsync(
                input,
                cancellationToken);

            return new SaveIdentityResourcePayload(identityResource);
        }
    }
}
