
using System.Collections.Generic;
using System.Linq;
using Duende.IdentityServer.Events;

namespace IdOps.IdentityServer.Events
{
    public class PersonalAccessTokenValidationSuccessEvent : Event
    {
        public PersonalAccessTokenValidationSuccessEvent(
            string clientId,
            string userName,
            string? scopes)
            : base(
                "PersonalAccessToken",
                "PAT validation Success",
                EventTypes.Success,
                IdOpsEventIds.PersonalAccessTokenValidationSuccess)
        {
            ClientId = clientId;
            UserName = userName;
            Scopes = scopes;
        }

        public string ClientId { get; }

        public string? Scopes { get; }

        public string UserName { get; }

        public static PersonalAccessTokenValidationSuccessEvent New(
            string clientId,
            string userName,
            IEnumerable<string>? scopes) => new(
                clientId,
                userName,
                string.Join(",", scopes ?? Enumerable.Empty<string>()));
    }
}
