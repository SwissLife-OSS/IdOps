using Duende.IdentityServer.Models;

namespace IdOps.IdentityServer.Model
{
    public class IdOpsIdentityResource : IdentityResource, IdOpsResource
    {
        public PublishSource Source { get; set; }

    }
}
