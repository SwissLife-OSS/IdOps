using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveUserClaimRulePayload
    {

        public SaveUserClaimRulePayload(UserClaimRule rule)
        {
            Rule = rule;
        }

        public UserClaimRule Rule { get; }
    }
}
