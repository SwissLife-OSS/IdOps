using System;

namespace IdOps.GraphQL
{
    public class RequestClientCredentialsTokenInput : RequestTokenInput
    {

        public bool SaveTokens { get; set; }

        public RequestClientCredentialsTokenInput(
            string authority,
            string clientId,
            string secretId,
            string grantType,
            bool saveTokens)
        {
            Authority = authority;
            ClientId = clientId;
            SecretId = secretId;
            GrantType = grantType;
            SaveTokens = saveTokens;
        }
    }
}
