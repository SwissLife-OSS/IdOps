using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdOps.IdentityServer.DataConnector
{
    public class UserDataConnectorResult
    {
        public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();

        public string? CacheKey { get; set; }

        public bool Executed { get; set; }

        public bool Success { get; set; }

        public Exception Error { get; set; }
    }
}
