namespace IdOps.IdentityServer.Storage.Mongo
{
    public class CollectionNames
    {
        public string Client { get; set; } = "client";

        public string ApiScope { get; set; } = "api_scope";

        public string IdentityResource { get; set; } = "identity_resource";

        public string ApiResource { get; set; } = "api_resource";

        public string UserClaimRule { get; set; } = "user_claim_rule";

        public string UserDataConnectorData { get; set; } = "user_connector_data";

        public string PersonalAccessTokens { get; set; } = "personal_access_tokens";
    }
}
