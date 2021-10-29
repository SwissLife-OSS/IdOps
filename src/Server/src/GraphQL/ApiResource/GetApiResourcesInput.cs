using System.Collections.Generic;

namespace IdOps.GraphQL
{
    public record GetApiResourcesInput(IReadOnlyList<string> Tenants);
}
