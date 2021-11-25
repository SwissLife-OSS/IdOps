using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public record UpdatePersonalAccessTokenRequest(
        Guid Id,
        string? UserName,
        string? Source,
        ICollection<Guid>? AllowedScopes,
        ICollection<Guid>? AllowedApplicationIds,
        ICollection<IdOpsClaimExtension>? ClaimsExtensions)
    {
    }
}
