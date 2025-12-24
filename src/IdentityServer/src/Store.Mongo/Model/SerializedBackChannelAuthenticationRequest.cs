using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Duende.IdentityServer.Models;

namespace IdOps.IdentityServer.Storage.Mongo.Model
{
    /// <summary>
    /// Serializable model for <see cref="BackChannelAuthenticationRequest"/>.
    /// The <see cref="ClaimsPrincipal"/> is serialized as a collection of claims.
    /// </summary>
    public class SerializedBackChannelAuthenticationRequest
    {
        /// <summary>
        /// The identifier for this request in the store.
        /// </summary>
        public string InternalId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the life time in seconds.
        /// </summary>
        public int Lifetime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the client.
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the subject claims (serialized from ClaimsPrincipal).
        /// </summary>
        public ICollection<SerializedClaim> SubjectClaims { get; set; } = new List<SerializedClaim>();

        /// <summary>
        /// Gets or sets the authentication type for the subject.
        /// </summary>
        public string? SubjectAuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the requested scopes.
        /// </summary>
        public IEnumerable<string> RequestedScopes { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the requested resource indicators.
        /// </summary>
        public IEnumerable<string> RequestedResourceIndicators { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the authentication context reference classes.
        /// </summary>
        public ICollection<string> AuthenticationContextReferenceClasses { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string? Tenant { get; set; }

        /// <summary>
        /// Gets or sets the idp.
        /// </summary>
        public string? IdP { get; set; }

        /// <summary>
        /// Gets or sets the binding message.
        /// </summary>
        public string? BindingMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been completed.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Gets or sets the authorized scopes.
        /// </summary>
        public IEnumerable<string> AuthorizedScopes { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the session identifier from which the user approved the request.
        /// </summary>
        public string? SessionId { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the client being authorized.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Calculated expiration time for TTL index.
        /// </summary>
        public DateTime ExpiresAt => CreationTime.AddSeconds(Lifetime);

        /// <summary>
        /// Converts from <see cref="BackChannelAuthenticationRequest"/> to serializable model.
        /// </summary>
        public static SerializedBackChannelAuthenticationRequest FromBackChannelAuthenticationRequest(
            BackChannelAuthenticationRequest request)
        {
            return new SerializedBackChannelAuthenticationRequest
            {
                InternalId = request.InternalId,
                CreationTime = request.CreationTime,
                Lifetime = request.Lifetime,
                ClientId = request.ClientId,
                SubjectClaims = request.Subject?.Claims
                    .Select(c => new SerializedClaim { Type = c.Type, Value = c.Value, ValueType = c.ValueType })
                    .ToList() ?? new List<SerializedClaim>(),
                SubjectAuthenticationType = request.Subject?.Identity?.AuthenticationType,
                RequestedScopes = request.RequestedScopes ?? Array.Empty<string>(),
                RequestedResourceIndicators = request.RequestedResourceIndicators ?? Array.Empty<string>(),
                AuthenticationContextReferenceClasses = request.AuthenticationContextReferenceClasses ?? new List<string>(),
                Tenant = request.Tenant,
                IdP = request.IdP,
                BindingMessage = request.BindingMessage,
                IsComplete = request.IsComplete,
                AuthorizedScopes = request.AuthorizedScopes ?? Array.Empty<string>(),
                SessionId = request.SessionId,
                Description = request.Description
            };
        }

        /// <summary>
        /// Converts to <see cref="BackChannelAuthenticationRequest"/>.
        /// </summary>
        public BackChannelAuthenticationRequest ToBackChannelAuthenticationRequest()
        {
            var claims = SubjectClaims
                .Select(c => new Claim(c.Type, c.Value, c.ValueType))
                .ToList();

            var identity = new ClaimsIdentity(claims, SubjectAuthenticationType);
            var principal = new ClaimsPrincipal(identity);

            return new BackChannelAuthenticationRequest
            {
                InternalId = InternalId,
                CreationTime = CreationTime,
                Lifetime = Lifetime,
                ClientId = ClientId,
                Subject = principal,
                RequestedScopes = RequestedScopes,
                RequestedResourceIndicators = RequestedResourceIndicators,
                AuthenticationContextReferenceClasses = AuthenticationContextReferenceClasses,
                Tenant = Tenant,
                IdP = IdP,
                BindingMessage = BindingMessage,
                IsComplete = IsComplete,
                AuthorizedScopes = AuthorizedScopes,
                SessionId = SessionId,
                Description = Description
            };
        }
    }

    /// <summary>
    /// Serializable claim.
    /// </summary>
    public class SerializedClaim
    {
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string ValueType { get; set; } = ClaimValueTypes.String;
    }
}
