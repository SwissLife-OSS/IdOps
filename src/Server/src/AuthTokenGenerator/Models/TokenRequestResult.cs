namespace IdOps.Models;

public record RequestTokenResult(bool IsSuccess)
{
    public TokenModel? AccessToken { get; set; }

    public string? ErrorMessage { get; set; }
}