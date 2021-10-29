using System;

namespace IdOps.Messages
{
    public class ResourcePublishedSuccessMessage
    {
        public Guid JobId { get; set; }

        public string ResourceType { get; set; }

        public Guid ResourceId { get; set; }

        public string Operation { get; set; }

        public int OldVersion { get; set; }

        public int NewVersion { get; set; }

        public string EnvironmentName { get; set; }

        public string RequestedBy { get; set; }

        public Guid IdentityServerGroupId { get; set; }

    }

    public class ResourcePublishedErrorMessage
    {
        public Guid JobId { get; set; }

        public string ResourceType { get; set; }

        public Guid ResourceId { get; set; }

        public string EnvironmentName { get; set; }

        public string RequestedBy { get; set; }

        public string ErrorMessage { get; set; }

        public Guid IdentityServerGroupId { get; set; }

    }
}
