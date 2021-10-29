namespace IdOps.IdentityServer.Events
{
    internal static class EventIds
    {
        public static readonly int ExternalAuthenticationSuccess = 9000;
        public static readonly int Authorize = 9001;
        public static readonly int DataConnectorSuccess = 9002;
        public static readonly int DataConnectorFailed = 9003;
        public static readonly int UserInfoSucess = 9004;
        public static readonly int CorsOriginNotAllowed = 9005;
        public static readonly int UriNotAllowed = 9006;
        public static readonly int OAuth2SamlBearerTokenSucess = 9007;
        public static readonly int OAuth2SamlBearerTokenFailed = 9008;
    }
}
