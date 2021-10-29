using System.Collections.Generic;

namespace IdOps.Model
{
    public class GrantType
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> Tenants { get; set; }

        public bool IsCustom { get; set; }
    }
}
