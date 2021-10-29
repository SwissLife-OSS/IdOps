using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public interface IResourceMessageConsumer
    {
        string MessageType { get; }

        Task<UpdateResourceResult> ProcessAsync(byte[] data, CancellationToken cancellationToken);
    }
}
