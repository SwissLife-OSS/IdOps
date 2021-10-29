using System;

namespace IdOps.Model
{
    public class Secret
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public string Value { get; set; }

        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = "SharedSecret";
    }
}
