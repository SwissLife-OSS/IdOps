using System;
using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public class UserDataConnectorFailedEvent : Event
    {
        public UserDataConnectorFailedEvent(
            string clientId,
            string subjectId,
            string name,
            string caller,
            Exception ex)
            : base(
                "DataConnector",
                "DataConnector failed",
                EventTypes.Failure,
                EventIds.DataConnectorFailed)
        {
            SubjectId = subjectId;
            ClientId = clientId;
            ConnectorName = name;
            Caller = caller;
            Message = ex.Message;
        }

        public string SubjectId { get; }

        public string ClientId { get; }

        public string ConnectorName { get; }

        public string Caller { get; }
    }
}
