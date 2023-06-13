using System.Collections.Generic;

namespace IdOps.Model;

public class IpAddressFilter
{
    public IpFilterPolicy Policy { get; set; } = IpFilterPolicy.Internal;
    public bool WarnOnly { get; set; }
    public ICollection<string> AllowList { get; set; } = new List<string>();
}
