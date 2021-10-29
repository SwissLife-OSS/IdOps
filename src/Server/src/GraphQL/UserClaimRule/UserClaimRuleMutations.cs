using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class UserClaimRuleMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveUserClaimRulePayload> SaveUserClaimRuleAsync(
            [Service] IUserClaimRulesService userClaimRulesService,
            SaveUserClaimRuleRequest input,
            CancellationToken cancellationToken)
        {
            UserClaimRule rule = await userClaimRulesService.SaveAsync(input, cancellationToken);

            return new SaveUserClaimRulePayload(rule);
        }
    }
}
