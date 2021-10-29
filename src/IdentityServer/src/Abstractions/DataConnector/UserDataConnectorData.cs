using System;
using System.Collections.Generic;

namespace IdOps.IdentityServer.DataConnector
{
    public class UserDataConnectorData
    {
        public string Key { get; set; }

        public string Connector { get; set; }

        public string SubjectId { get; set; }

        public IEnumerable<ClaimData>  Claims { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }

    public class ClaimData
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }
}
