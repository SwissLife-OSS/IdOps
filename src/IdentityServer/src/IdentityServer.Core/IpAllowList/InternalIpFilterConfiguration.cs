using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IdOps.IdentityServer;

public class InternalIpFilterConfiguration
{
    public InternalIpFilterConfiguration()
    {
        InternalIpAllowListParsed = new Lazy<IList<IPAddress>>(ParsedIpAllowList);
    }

    private IList<IPAddress> ParsedIpAllowList()
    {
        return InternalIpAllowList.Select(
                a => IPAddress.TryParse(a, out IPAddress? ipAddress) ? ipAddress : null)
            .OfType<IPAddress>().ToList();
    }

    public ICollection<string> InternalIpAllowList { get; set; } = new List<string>();

    public Lazy<IList<IPAddress>> InternalIpAllowListParsed;
}
