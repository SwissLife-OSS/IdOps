using Duende.IdentityServer.Models;

namespace IdOps.IdentityServer.Model
{
    public class IdOpsApiScope : ApiScope, IdOpsResource
    {
        public PublishSource Source { get; set; }
    }
}
