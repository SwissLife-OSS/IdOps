using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public class ApiResourceImport : ApiResource
    {
        public new ICollection<string> Scopes { get; set; }
    }

    public class ClientImport : Client
    {
        public ICollection<string> AllowedScopes { get; set; }

        public string ClientName { get; set; }

        public new ICollection<string> Environments { get; set; }
    }
}
