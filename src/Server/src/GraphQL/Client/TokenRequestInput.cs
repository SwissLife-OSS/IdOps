using System;
using System.Collections.Generic;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps.GraphQL
{
    public record TokenRequestInput(
        string Authority,
        Guid ClientId,
        Guid SecretId,
        IEnumerable<TokenRequestParameter> Parameters,
        bool SaveTokens,
        Guid? RequestId);
}
