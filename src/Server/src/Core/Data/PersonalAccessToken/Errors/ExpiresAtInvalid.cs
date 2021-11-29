using System;

namespace IdOps
{
    /// <summary>
    /// A error message that occurs when the `expiresAt` field of a token was in the past
    /// </summary>
    public record ExpiresAtInvalid(DateTime ExpiresAt)
        : Error($"The provided value for `expiresAt` was invalid."),
          ICreatePersonalAccessTokenError;
}
