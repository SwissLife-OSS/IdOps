using System;

namespace IdOps.Model
{
    public class ResourcePublishState
    {
        public Guid Id { get; set; }

        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public Guid EnvironmentId { get; set; }

        public int Version { get; set; }

        public DateTime PublishedAt { get; set; }
    }

    public class ResourcePublishLog
    {
        public Guid Id { get; set; }

        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public Guid EnvironmentId { get; set; }

        public Guid IdentityServerGroupId { get; set; }

        public int Version { get; set; }

        public DateTime PublishedAt { get; set; }

        public string RequestedBy { get; set; }

        public string Operation { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class ResourceApprovalState
    {
        public Guid Id { get; set; }

        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public Guid EnvironmentId { get; set; }

        public int Version { get; set; }

        public DateTime ApprovedAt { get; set; }
    }

    public class ResourceApprovalLog
    {
        public Guid Id { get; set; }

        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public Guid EnvironmentId { get; set; }

        public int Version { get; set; }

        public DateTime ApprovedAt { get; set; }

        public string RequestedBy { get; set; }

        public string Operation { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
