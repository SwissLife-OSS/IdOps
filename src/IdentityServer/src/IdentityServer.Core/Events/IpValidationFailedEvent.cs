using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events;

public class IpValidationFailedEvent : Event
{
    public IpValidationFailedEvent(
        string clientId,
        string message) : base(
        "Ip Validation",
        "Ip Validation failed",
        EventTypes.Failure,
        IdOpsEventIds.IpValidationFailed,
        message)
    {
        ClientId = clientId;
    }

    public string ClientId { get; }

    public static IpValidationFailedEvent New(
        string clientId,
        string message) => new(clientId, message);
}
