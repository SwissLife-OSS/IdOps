using System.Collections.Generic;

namespace IdOps.GraphQL
{
    public record GetPersonalAccessTokensInput(IReadOnlyList<string> Tenants);
}
