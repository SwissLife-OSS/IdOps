using System.Collections.Generic;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class AddEnvironmentToApplicationPayload
    {
        public AddEnvironmentToApplicationPayload(
            Application application, IEnumerable<CreatedClientInfo> clients)
        {
            Application = application;
            Clients = clients;
        }

        public Application? Application { get; }
        public IEnumerable<CreatedClientInfo> Clients { get; }
    }
}
