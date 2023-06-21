namespace IdOps.Models;

public record TokenRequestData(
    string Authority,
    string ClientId,
    string Secret,
    string GrantType,
    IEnumerable<string> Scopes)
{
    public bool SaveTokens { get; init; }
}
