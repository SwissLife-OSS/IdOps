using System;

namespace IdOps.Model
{
    public class ClientTemplateSecret
    {
        public string? Type { get; set; } = "SharedSecret";

        public string? Value { get; set; }

        public Guid EnvironmentId { get; set; }
    }
}

