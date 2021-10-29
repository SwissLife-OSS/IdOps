using System;
using System.Collections.Generic;

namespace IdOps
{
    public record ResourceApprovalRequest
    {
        public IReadOnlyList<Guid>? ResourceIds { get; set; }

        public IReadOnlyList<string>? ResourceTypes { get; set; }

        public Guid? EnvironmentId { get; set; }

        public Guid? IdentityServerGroupId { get; set; }

        public IReadOnlyList<string>? Tenants { get; set; } = Array.Empty<string>();
    }

    public record ApproveResourcesRequest(IEnumerable<ApproveResource> Resources);
    public record ApproveResource(Guid ResourceId, string Type, Guid EnvironmentId, int Version);

    public record ApproveResourcesResult(IEnumerable<Guid> Resources)
    {
        public static readonly ApproveResourcesResult Empty =
            new(Array.Empty<Guid>());
    }
}
