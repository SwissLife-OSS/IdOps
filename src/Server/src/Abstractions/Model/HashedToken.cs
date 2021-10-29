using System;

namespace IdOps.Model
{
    public class HashedToken
    {
        public HashedToken(
            Guid id,
            string token,
            DateTime expiresAt,
            DateTime createdAt)
        {
            Id = id;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = createdAt;
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
    }
}
