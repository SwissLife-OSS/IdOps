using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public interface IResourceConsumer
    {
        Task<UpdateResourceResult> ProcessAsync(
            string messageType,
            byte[] data,
            CancellationToken cancellationToken);
    }
}
