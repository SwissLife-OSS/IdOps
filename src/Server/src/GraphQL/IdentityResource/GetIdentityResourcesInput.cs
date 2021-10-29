using System.Collections.Generic;

namespace IdOps.GraphQL
{
    public record GetIdentityResourcesInput(IReadOnlyList<string> Tenants);
}
