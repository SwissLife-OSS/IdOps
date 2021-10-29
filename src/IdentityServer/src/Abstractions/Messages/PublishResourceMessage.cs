using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
