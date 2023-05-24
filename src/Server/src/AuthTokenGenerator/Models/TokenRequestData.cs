namespace IdOps.Models;

public record TokenRequestData(
    string Authority,
    string ClientId,
    string Secret,
    string GrantType,
    IEnumerable<string> Scopes,
    IEnumerable<TokenRequestParameter> Parameters)
{
    public bool SaveTokens { get; init; }

    public Guid? RequestId { get; init; }
}
