using System.Collections.Generic;

namespace IdOps.Model
{
    public class Dependency
    {
        public IEnumerable<ApiScope>? ApiScopes { get; set; } = new HashSet<ApiScope>();

        public IEnumerable<IdentityResource>? IdentityResources { get; set; } = new HashSet<IdentityResource>();

        public IEnumerable<ApiResource>? ApiResources { get; set; } = new HashSet<ApiResource>();

    }
}
