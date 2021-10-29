using IdOps.Model;

namespace IdOps.GraphQL
{
    /// <summary>
    /// The payload object for `createPersonalAccessToken`
    /// </summary>
    /// <param name="Token">The token that was created. Is null if there are any errors.</param>
    /// <param name="Errors">Possibly errors of the request</param>
    public record CreatePersonalAccessTokenPayload(
        PersonalAccessToken? Token,
        params ICreatePersonalAccessTokenError[] Errors);
}
