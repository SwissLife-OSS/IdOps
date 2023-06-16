using System.Collections.Generic;

namespace IdOps.Model;

public class IpAddressFilter
{
    public IpFilterPolicy Policy { get; set; } = IpFilterPolicy.Public;
    public bool WarnOnly { get; set; } = true;
    public ICollection<string> AllowList { get; set; } = new List<string>();
}
