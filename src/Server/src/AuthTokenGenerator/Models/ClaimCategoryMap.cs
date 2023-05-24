namespace IdOps.Models;

public record ClaimCategoryMap(string Type, ClaimCategory Category)
{
    public bool Hide { get; init; }
}
