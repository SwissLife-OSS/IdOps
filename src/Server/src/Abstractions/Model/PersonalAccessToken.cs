using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    /// <summary>
    /// A personal access token for one login that can be used to authenticate on the identity
    /// server with the personal access token flow
    /// </summary>
    public class PersonalAccessToken : ITenantResource
    {
        /// <summary>
        /// The id of the access token
        /// </summary>
        public Guid Id { get; set; }

        public string Title => UserName;

        public ResourceVersion Version { get; set; }

        public bool IsInServerGroup(IdentityServerGroup serverGroup)
        {
            return serverGroup.Tenants.Contains(Tenant);
        }

        /// <summary>
        /// The username of the user this access token belongs to
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The Environment where this token is deployed
        /// </summary>
        public Guid EnvironmentId { get; set; }

        /// <summary>
        /// The encrypted access tokens
        /// </summary>
        public ICollection<HashedToken> Tokens { get; set; }

        /// <summary>
        /// The timestamp of the creation of this access token
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The scoped that are allowed to be requested with the access token
        /// </summary>
        public ICollection<string> AllowedScopes { get; set; }

        /// <summary>
        /// The clients that are allowed to request this access token
        /// </summary>
        public ICollection<Guid> AllowedApplicationIds { get; set; }

        /// <summary>
        /// The Source of this token
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The kind of hasher that was use to hash the token
        /// </summary>
        public string HashAlgorithm { get; set; }

        /// <summary>
        /// A list of extensions for this claim extensions of this access token. These extensions
        /// will be added to the token when it is issued.
        /// </summary>
        public ICollection<IdOpsClaimExtension> ClaimsExtensions { get; set; }

        public string Tenant { get; set; }

        public bool RequiresApproval(Guid id) => true;

        public ICollection<Guid> GetEnvironmentIds() =>
            _environmentIds ??= new[]
            {
                EnvironmentId
            };

        private ICollection<Guid>? _environmentIds;

    }
}
