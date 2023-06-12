using System.Collections.Generic;

namespace IdOps.IdentityServer;

public class InternalIpFilterConfiguration
{
    public ICollection<string> InternalIpAllowList { get; set; } = new List<string>();
}
