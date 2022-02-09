using System.Threading.Tasks;
using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public interface IIdOpsEventSink
    {
        Task ProcessAsync(Event evt);
    }
}
