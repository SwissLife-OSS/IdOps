namespace IdOps.Models;

public record TokenClaim(string Type, string Value)
{
    public ClaimCategory Category { get; init; }
}
