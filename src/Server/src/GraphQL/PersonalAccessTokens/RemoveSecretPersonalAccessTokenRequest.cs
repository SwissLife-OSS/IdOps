using System;

namespace IdOps.GraphQL
{
    public class RemoveSecretPersonalAccessTokenRequest
    {
        public RemoveSecretPersonalAccessTokenRequest(
            Guid tokenId,
            Guid secretId)
        {
            TokenId = tokenId;
            SecretId = secretId;
        }

        public Guid TokenId { get; }

        public Guid SecretId { get; }
    }
}
