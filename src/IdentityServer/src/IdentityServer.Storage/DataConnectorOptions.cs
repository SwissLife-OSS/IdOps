using System.Collections.Generic;

namespace IdOps.IdentityServer.Model
{
    public class DataConnectorOptions
    {
        public string? Name { get; set; }

        public bool Enabled { get; set; }

        public IEnumerable<ConnectorProfileType>? ProfileTypeFilter { get; set; }

        public IEnumerable<DataConnectorProperty>? Properties { get; set; }
    }
}
