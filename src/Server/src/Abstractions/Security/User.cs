using System.Collections.Generic;

namespace IdOps.Security
{
    public record User(
        string Id,
        string Name,
        IReadOnlyList<string> Roles)
    {
    }
}
