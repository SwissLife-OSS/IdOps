using System.Collections.Generic;

namespace IdOps.GraphQL
{
    public record GetUserClaimRulesInput(IReadOnlyList<string> Tenants);
}
