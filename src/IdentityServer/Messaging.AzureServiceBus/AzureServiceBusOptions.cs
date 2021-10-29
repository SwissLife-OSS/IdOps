using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdOps.IdentityServer.AzureServiceBus
{
    public class AzureServiceBusOptions
    {
        public string ConnectionString { get; set; } = default!;

        public int PrefetchCount { get; set; } = 10;
    }
}
