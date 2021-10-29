using System.Collections.Generic;
using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace IdOps.IdentityServer
{
    /// <summary>
    /// Context data that can be used through the
    /// </summary>
    public class PersonalAccessTokenValidationContext
    {
        public PersonalAccessTokenValidationContext(
            IEnumerable<string> requestedScopes,
            string requestedResourceIndicator,
            string userName,
            Client client,
            ClaimsPrincipal subject,
            string sessionId,
            string token)
        {
            RequestedScopes = requestedScopes;
            RequestedResourceIndicator = requestedResourceIndicator;
            UserName = userName;
            Client = client;
            Subject = subject;
            SessionId = sessionId;
            Token = token;
        }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        public IEnumerable<string> RequestedScopes { get; }

        /// <summary>
        /// Gets or sets the resource indicator.
        /// </summary>
        public string RequestedResourceIndicator { get; }

        /// <summary>
        /// Gets or sets the username used in the request.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets or sets the secret used in the request.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public ClaimsPrincipal Subject { get; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public string SessionId { get; }

        public static PersonalAccessTokenValidationContext From(
            ExtensionGrantValidationContext context,
            string token) =>
            new(context.Request.RequestedScopes,
                context.Request.RequestedResourceIndicator,
                context.Request.UserName,
                context.Request.Client,
                context.Request.Subject,
                context.Request.SessionId,
                token);
    }
}
