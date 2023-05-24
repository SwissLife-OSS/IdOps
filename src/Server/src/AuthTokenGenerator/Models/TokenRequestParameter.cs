namespace IdOps.Models;

public record TokenRequestParameter(string Name)
{
    public string? Value { get; set; }
}