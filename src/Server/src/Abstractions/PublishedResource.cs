using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public record PublishedResource(
        Guid Id,
        string Title,
        string Type,
        ResourceVersion CurrentVersion)
    {
        public ICollection<PublishedResourceEnvironment> Environments { get; set; }
            = new List<PublishedResourceEnvironment>();
    }

    public record PublishedResourceEnvironment(Guid EnvironmentId)
    {
        public DateTime? ApprovedAt { get; init; }
        public int? Version { get; init; }
        public DateTime? PublishedAt { get; init; }
        public string? State { get; set; }
    }

    public record PublishedResourcesRequest
    {
        public Guid? Environment { get; set; }

        public IReadOnlyList<Guid>? ResourceId { get; set; }

        public IReadOnlyList<string>? ResourceTypes { get; set; }

        public Guid? IdentityServerGroupId { get; set; }

        public IReadOnlyList<string> Tenants { get; set; } = Array.Empty<string>();
    }
}
