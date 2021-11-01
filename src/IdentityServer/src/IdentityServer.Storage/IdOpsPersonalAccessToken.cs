using System;
using System.Collections.Generic;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Model
{
    /// <summary>
    /// A personal access token for one login that can be used to authenticate on the identity
    /// server with the personal access token flow
    /// </summary>
    public record IdOpsPersonalAccessToken : IdOpsResource
    {
        public PublishSource Source { get; set; }

        /// <summary>
        /// The id of the access token
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The username of the user this access token belongs to
        /// </summary>
        public string UserName { get; init; }

        /// <summary>
        /// The encrypted access tokens
        /// </summary>
        public List<IdOpsHashedToken> Tokens { get; set; }

        /// <summary>
        /// The timestamp of the creation of this access token
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// If the token is a one time token or if the token can be used multiple times
        /// </summary>
        public bool IsOneTime { get; init; }

        /// <summary>
        /// The scoped that are allowed to be requested with the access token
        /// </summary>
        public ICollection<string> AllowedScopes { get; init; }

        /// <summary>
        /// The clients that are allowed to request this access token
        /// </summary>
        public ICollection<string> AllowedClients { get; init; }

        /// <summary>
        /// The Source of this token
        /// </summary>
        public string TokenSource { get; init; }

        /// <summary>
        /// The kind of algorithm that was use to hash the token
        /// </summary>
        public string HashAlgorithm { get; init; }

        /// <summary>
        /// A list of extensions for this claim extensions of this access token. These extensions
        /// will be added to the token when it is issued.
        /// </summary>
        public IReadOnlyList<ClaimExtension> ClaimExtensions { get; init; }
            = Array.Empty<ClaimExtension>();
    }
}
