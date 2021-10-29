using System;

namespace IdOps.Model
{
    public class ClientScope
    {
        public ScopeType Type { get; set; }

        public Guid Id { get; set; }
    }

    public enum ScopeType
    {
        Identity,
        Resource
    }
}
