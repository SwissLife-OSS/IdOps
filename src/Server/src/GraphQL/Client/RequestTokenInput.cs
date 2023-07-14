using System;

namespace IdOps.GraphQL
{
    public record RequestTokenInput(
        string Authority, 
        Guid ClientId, 
        Guid SecretId,
        string grantType,
        bool SaveTokens,
        string? code);
}
