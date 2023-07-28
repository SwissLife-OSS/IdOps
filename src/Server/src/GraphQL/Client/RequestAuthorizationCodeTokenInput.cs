namespace IdOps.GraphQL;

public class RequestAuthorizationCodeTokenInput : RequestTokenInput
{
    public string Code { get; set; }
    public string Verifier { get; set; }
    public string RedirectUri { get; set; }

    public RequestAuthorizationCodeTokenInput(
        string authority,
        string clientId,
        string secretId, 
        string grantType, 
        string code, 
        string verifier, 
        string redirectUri)
    {
        Authority = authority;
        ClientId = clientId;
        SecretId = secretId;
        GrantType = grantType;
        Code = code;
        Verifier = verifier;
        RedirectUri = redirectUri;
    }
}
