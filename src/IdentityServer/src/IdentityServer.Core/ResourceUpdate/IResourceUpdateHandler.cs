using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Messages;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public interface IResourceUpdateHandler
    {
        Task<UpdateResourceResult> HandleUpdateAsync(PublishResourceMessage update, CancellationToken cancellationToken);
    }
}
