using IdOps.Model;

namespace IdOps
{
    /// <summary>
    /// The payload object for `createPersonalAccessToken`
    /// </summary>
    /// <param name="Token">The token that was created. Is null if there are any errors.</param>
    public record CreatePersonalAccessTokenResult(
        PersonalAccessToken Token);
}
