using IdOps.Messages;
using IdOps.Model;

namespace IdOps.Consumers
{
    public interface IIdentityServerEventMapper
    {
        IdentityServerEvent? CreateEvent(IdentityEventMessage message);
    }
}