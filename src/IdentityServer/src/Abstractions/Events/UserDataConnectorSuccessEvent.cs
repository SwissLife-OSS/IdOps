using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public class UserDataConnectorSuccessEvent : Event
    {
        public UserDataConnectorSuccessEvent(
            string clientId,
            string? subjectId,
            string name,
            string caller,
            int claimsCount)
            : base(
                "DataConnector",
                "DataConnector executed",
                EventTypes.Success,
                EventIds.DataConnectorSuccess)
        {
            SubjectId = subjectId;
            ClientId = clientId;
            ConnectorName = name;
            Caller = caller;
            ClaimsCount = claimsCount;
        }

        public string? SubjectId { get; }

        public string ClientId { get; }

        public string ConnectorName { get; }

        public string Caller { get; }

        public int ClaimsCount { get; }
    }
}
