using System.Collections.Generic;
using Duende.IdentityServer.Models;

namespace IdOps.IdentityServer.Model
{
    public class IdOpsClient : Client, IdOpsResource
    {
        public IEnumerable<EnabledProvider> EnabledProviders { get; set; }

        public IEnumerable<DataConnectorOptions> DataConnectors { get; set; }

        public string DisplayName { get; set; }

        public string Tenant { get; set; }

        public PublishSource Source { get; set; }

    }
}
