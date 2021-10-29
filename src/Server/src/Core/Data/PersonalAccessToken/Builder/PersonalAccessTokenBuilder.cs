using System;
using System.Collections.Generic;
using IdOps.IdentityServer.Hashing;
using IdOps.Model;

namespace IdOps.Builder
{
    public class PersonalAccessTokenBuilder
    {
        private readonly List<HashedToken> _tokens = new();
        private readonly HashSet<string> _allowedScopes = new();
        private readonly HashSet<Guid> _allowedApplications = new();
        private readonly List<IdOpsClaimExtension> _claimExtensions = new();

        private Guid? _id;
        private Guid? _environmentId;
        private string? _userName;
        private DateTime _createdAt = DateTime.UtcNow;
        private string _source = WellKnownPersonalAccessTokenSources.Local;
        private string _hashAlgorithm = SshaHashAlgorithm.Identifier;
        private string _tenant;

        public PersonalAccessTokenBuilder SetUserName(string? userName)
        {
            if (userName is { } && string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("UserName cannot be empty");
            }

            _userName = userName;

            return this;
        }

        public PersonalAccessTokenBuilder SetId(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty");
            }

            _id = id;

            return this;
        }

        public PersonalAccessTokenBuilder SetEnvironment(Guid? environmentId)
        {
            if (environmentId == null || environmentId == Guid.Empty)
            {
                throw new ArgumentException("EnvironmentId cannot be empty");
            }

            _environmentId = environmentId;

            return this;
        }

        public PersonalAccessTokenBuilder SetAllowedApplications(
            IEnumerable<Guid> allowedApplications)
        {
            foreach (Guid allowedApplication in allowedApplications)
            {
                _allowedApplications.Add(allowedApplication);
            }

            return this;
        }

        public PersonalAccessTokenBuilder SetAllowedScopes(IEnumerable<string> allowedScopes)
        {
            foreach (var allowedClient in allowedScopes)
            {
                _allowedScopes.Add(allowedClient);
            }

            return this;
        }

        public PersonalAccessTokenBuilder SetHashAlgorithm(string hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
            return this;
        }

        public PersonalAccessTokenBuilder SetCreatedAt(DateTime createdAt)
        {
            if (createdAt == DateTime.MinValue)
            {
                throw new ArgumentException(
                    "CreatedAt cannot be left as the default value, " +
                    "it must have a value assigned");
            }

            _createdAt = createdAt;

            return this;
        }

        public PersonalAccessTokenBuilder SetSource(string sourceKind)
        {
            _source = sourceKind;

            return this;
        }

        public PersonalAccessTokenBuilder SetTenant(string tenant)
        {
            _tenant = tenant;

            return this;
        }

        public PersonalAccessTokenBuilder AddClaimExtension(string type, string value)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));
            _ = value ?? throw new ArgumentNullException(nameof(value));

            _claimExtensions.Add(new IdOpsClaimExtension(type, value));

            return this;
        }

        public PersonalAccessToken Build()
        {
            _ = _id ?? throw new InvalidOperationException("Id cannot be null.");
            _ = _userName ?? throw new InvalidOperationException("UserName cannot be null.");
            _ = _environmentId ??
                throw new InvalidOperationException("EnvironmentId cannot be null.");
            _ = _source ?? throw new InvalidOperationException("Source cannot be null.");
            _ = _tenant ?? throw new InvalidOperationException("Tenant cannot be null.");

            return new PersonalAccessToken
            {
                Id = _id.Value,
                Source = _source,
                Tokens = _tokens,
                AllowedScopes = _allowedScopes,
                ClaimsExtensions = _claimExtensions,
                CreatedAt = _createdAt,
                EnvironmentId = _environmentId.Value,
                HashAlgorithm = _hashAlgorithm,
                UserName = _userName,
                Tenant = _tenant,
                AllowedApplicationIds = _allowedApplications
            };
        }

        public static PersonalAccessTokenBuilder New() => new();
    }
}
