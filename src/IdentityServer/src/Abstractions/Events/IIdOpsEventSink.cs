using System.Diagnostics;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public interface IIdOpsEventSink
    {
        ValueTask ProcessAsync(Event evt, Activity? activity);
    }
}
