using System;
using System.Collections.Generic;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps.GraphQL
{
    public record RequestTokenInput(
        string Authority, 
        Guid ClientId, 
        Guid SecretId,
        string grantType,
        bool SaveTokens);
}
