using System;
using System.Collections.Generic;

namespace IdOps
{
    /// <summary>
    /// The input object for `createPersonalAccessToken`
    /// </summary>
    /// <param name="UserName">
    /// The username this access token should belong to
    /// </param>
    /// <param name="EnvironmentId">
    /// The environments where this access token should be published
    /// </param>
    /// <param name="AllowedScopes">
    /// The scoped that are allowed to be requested with the access token
    /// </param>
    /// <param name="AllowedApplicationIds">
    /// The clients that are allowed to request this access token
    /// </param>
    /// <param name="ClaimsExtensions">
    /// A list of extensions for this claim extensions of this access token. These extensions
    /// will be added to the token when it is issued.
    /// </param>
    public record CreatePersonalAccessTokenRequest(
        string UserName,
        Guid EnvironmentId,
        string Tenant,
        string Source,
        string HashAlgorithm,
        IReadOnlyList<Guid> AllowedApplicationIds,
        IReadOnlyList<string> AllowedScopes,
        IReadOnlyList<ClaimsExtensionRequest> ClaimsExtensions) : ITenantInput;
}
