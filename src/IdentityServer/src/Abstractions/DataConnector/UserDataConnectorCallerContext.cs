using System.Collections.Generic;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.DataConnector
{
    public class UserDataConnectorCallerContext
    {
        public IdOpsClient? Client { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> RequestedClaimTypes { get; set; }
        public string Subject { get; set; }
    }
}
