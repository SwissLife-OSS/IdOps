using System;

namespace IdOps.IdentityServer.Model
{
    public record IdOpsHashedToken
    {
        public IdOpsHashedToken(
            Guid id,
            string token,
            DateTime expiresAt,
            DateTime createdAt,
            bool isUsed)
        {
            Id = id;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = createdAt;
            IsUsed = isUsed;
        }

        public IdOpsHashedToken()
        {
        }

        /// <summary>
        /// The id of the secret
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The hashed secret
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        /// The timestamp of the expiry of this access token. After this threshold is reached, no
        /// authentication with this access token will be possible
        /// </summary>
        public DateTime ExpiresAt { get; init; }

        /// <summary>
        /// The timestamp of the creation of this access token
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// If the token is already used or if it is still valid. If this field is false the token
        /// cannot be used for authentication any more
        /// </summary>
        public bool IsUsed { get; init; }
    }
}
