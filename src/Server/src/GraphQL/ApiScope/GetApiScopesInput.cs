using System.Collections.Generic;

namespace IdOps.GraphQL
{
    public record GetApiScopesInput(IReadOnlyList<string> Tenants);
}
