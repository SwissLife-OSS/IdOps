using System;

namespace IdOps.Messages
{
    public class PublishResourceMessage
    {
        public Guid JobId { get; set; }

        public Guid ResourceId { get; set; }

        public string ResourceType { get; set; }

        public byte[] Data { get; set; }

        public string RequestedBy { get; set; }

        public Guid IdentityServerGroupId { get; set; }
    }


    public class IdentityEventMessage
    {
        public string EnvironmentName { get; set; }

        public string Type { get; set; }

        public string Hostname { get; set; }

        public byte[] Data { get; set; }

        public string ServerGroup { get; set; }
    }
}
