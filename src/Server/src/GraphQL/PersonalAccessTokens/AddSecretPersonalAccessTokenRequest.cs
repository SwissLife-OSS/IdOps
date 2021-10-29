using System;

namespace IdOps.GraphQL
{
    public class AddSecretPersonalAccessTokenRequest
    {
        public AddSecretPersonalAccessTokenRequest(Guid tokenId, DateTime expiresAt)
        {
            TokenId = tokenId;
            ExpiresAt = expiresAt;
        }

        public Guid TokenId { get; }

        public DateTime ExpiresAt { get; }
    }
}
