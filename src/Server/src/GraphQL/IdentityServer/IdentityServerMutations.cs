using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class IdentityServerMutations
    {
        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.IdentityServerManage)]
        public async Task<SaveIdentityServerPayload> SaveIdentityServerAsync(
            SaveIdentityServerRequest input,
            [Service] IIdentityServerService identityServerService,
            CancellationToken cancellationToken)
        {
            Model.IdentityServer sever =
                await  identityServerService .SaveAsync(input, cancellationToken);

            return new SaveIdentityServerPayload(sever);
        }

        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.IdentityServerManage)]
        public async Task<SaveIdentityServerGroupPayload> SaveIdentityServerGroupAsync(
            SaveIdentityServerGroupRequest input,
            [Service] IIdentityServerGroupService identityServerGroupService,
            CancellationToken cancellationToken)
        {
            IdentityServerGroup severGroup =
                await  identityServerGroupService.SaveAsync(input, cancellationToken);

            return new SaveIdentityServerGroupPayload(severGroup);
        }
    }
}
