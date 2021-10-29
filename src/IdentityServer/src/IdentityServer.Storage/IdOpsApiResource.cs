using Duende.IdentityServer.Models;

namespace IdOps.IdentityServer.Model
{
    public class IdOpsApiResource : ApiResource, IdOpsResource
    {
        public PublishSource Source { get; set; }
    }

    public interface IdOpsResource
    {
        PublishSource Source { get; set; }
    }
}
