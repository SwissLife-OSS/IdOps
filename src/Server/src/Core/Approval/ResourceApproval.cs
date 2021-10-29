using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps
{
    public record ResourceApproval(
        Guid Id,
        string Title,
        string Type,
        ResourceVersion CurrentVersion)
    {
        public ICollection<ResourceApprovalEnvironment> Environments { get; set; }
            = new List<ResourceApprovalEnvironment>();
    }

    public record ResourceApprovalEnvironment(
        Guid EnvironmentId,
        int? Version,
        DateTime? ApprovedAt,
        string? State)
    {
        public static ResourceApprovalEnvironment NotApproved(Guid environmentId) =>
            new(environmentId, null, null, ResourceStates.NotApproved);

        public static ResourceApprovalEnvironment From(
            Guid environmentId,
            int? version,
            DateTime? approvedAt,
            string? state) =>
            new(environmentId, version, approvedAt, state);
    }
}
