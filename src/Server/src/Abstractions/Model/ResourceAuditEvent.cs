using System;
using System.Collections.Generic;

namespace IdOps.Model
{
    public class ResourceAuditEvent
    {
        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public string Action { get; set; }

        public DateTime Timestamp { get; set; }

        public IEnumerable<ResourceChange> Changes { get; set; }
        public int Version { get; set; }
        public Guid Id { get; set; }
        public string UserId { get; set; }
    }

    public class ResourceChange
    {
        public string Property { get; set; }

        public string Path { get; set; }

        public string? Before { get; set; }

        public string? After { get; set; }

        public string? Delta { get; set; }

        public int? ArrayIndex { get; set; }
    }
}
