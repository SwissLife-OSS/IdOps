using System;

namespace IdOps.IdentityServer.Model
{
    public class PublishSource
    {
        public Guid Id { get; set; }

        public DateTime ModifiedAt { get; set; }

        public int Version { get; set; }
    }
}
