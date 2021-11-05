using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Builder;
using IdOps.Configuration;
using IdOps.IdentityServer.Hashing;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class PersonalAccessTokenService
        : TenantResourceService<PersonalAccessToken>
        , IPersonalAccessTokenService
    {
        private readonly IHashAlgorithmResolver _resolver;
        private readonly IPasswordProvider _passwordProvider;
        private readonly IResourceManager<PersonalAccessToken> _resourceManager;
        private readonly IPersonalAccessTokenStore _store;

        public PersonalAccessTokenService(
            IdOpsServerOptions options,
            IPersonalAccessTokenStore store,
            IHashAlgorithmResolver resolver,
            IPasswordProvider passwordProvider,
            IResourceManager<PersonalAccessToken> resourceManager,
            IUserContextAccessor userContextAccessor)
            : base(options, userContextAccessor, store)
        {
            _store = store;
            _resolver = resolver;
            _passwordProvider = passwordProvider;
            _resourceManager = resourceManager;
        }

        public async Task<SearchResult<PersonalAccessToken>> SearchAsync(
            SearchPersonalAccessTokensRequest request,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<string> tenants = await GetUserTenantsAsync(cancellationToken);
            request = request with
            {
                Tenants = tenants.Intersect(request.Tenants ?? Array.Empty<string>())
            };
            return await _store.SearchAsync(request, cancellationToken);
        }

        public Task<PersonalAccessToken> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            _store.GetByIdAsync(id, cancellationToken);

        public async Task<AddSecretPersonalAccessTokenResult> AddSecretToTokenAsync(
            DateTime expiresAt,
            Guid id,
            CancellationToken cancellationToken)
        {
            PersonalAccessToken token =
                await _resourceManager.GetExistingOrCreateNewAsync(id, cancellationToken);

            await ValidateTenantAccess(token, cancellationToken);

            if (DateTime.UtcNow >= expiresAt)
            {
                ErrorException.Throw(new ExpiresAtWasInThePast(expiresAt));
            }

            if (!_resolver.TryResolve(token.HashAlgorithm, out IHashAlgorithm? encryptor))
            {
                ErrorException.Throw(new HashAlgorithmNotFound(token.HashAlgorithm));
            }

            string secret = _passwordProvider.GenerateRandomPassword(64);
            string hash = encryptor.ComputeHash(secret);
            token.Tokens.Add(new(Guid.NewGuid(), hash, expiresAt, DateTime.UtcNow));

            SaveResourceResult<PersonalAccessToken> result =
                await _resourceManager.SaveAsync(token, cancellationToken);

            return new AddSecretPersonalAccessTokenResult(result.Resource, secret);
        }

        public async Task<RemoveSecretPersonalAccessTokenResult> RemoveSecretOfTokenAsync(
            Guid id,
            Guid tokenId,
            CancellationToken cancellationToken)
        {
            PersonalAccessToken tokenResource =
                await _resourceManager.GetExistingOrCreateNewAsync(id, cancellationToken);

            await ValidateTenantAccess(tokenResource, cancellationToken);

            HashedToken? token = tokenResource.Tokens.FirstOrDefault(x => x.Id == tokenId);
            if (token is null)
            {
                return new RemoveSecretPersonalAccessTokenResult(null, null);
            }

            tokenResource.Tokens.Remove(token);

            SaveResourceResult<PersonalAccessToken> result =
                await _resourceManager.SaveAsync(tokenResource, cancellationToken);

            return new RemoveSecretPersonalAccessTokenResult(result.Resource, token);
        }

        public async Task<CreatePersonalAccessTokenResult> CreateAsync(
            CreatePersonalAccessTokenRequest request,
            CancellationToken cancellationToken)
        {
            PersonalAccessTokenBuilder builder = PersonalAccessTokenBuilder
                .New()
                .SetUserName(request.UserName)
                .SetId(Guid.NewGuid())
                .SetEnvironment(request.EnvironmentId)
                .SetSource(request.Source)
                .SetTenant(request.Tenant)
                .SetAllowedApplications(request.AllowedApplicationIds)
                .SetAllowedScopes(request.AllowedScopes);

            foreach (var (type, value) in request.ClaimsExtensions)
            {
                builder.AddClaimExtension(type, value);
            }

            PersonalAccessToken token = builder.Build();

            await ValidateTenantAccess(token, cancellationToken);

            _resourceManager.SetNewVersion(token);

            SaveResourceResult<PersonalAccessToken> result =
                await _resourceManager.SaveAsync(token, cancellationToken);

            return new CreatePersonalAccessTokenResult(result.Resource);
        }

        public async Task<PersonalAccessToken> UpdateAsync(
            UpdatePersonalAccessTokenRequest request,
            CancellationToken cancellationToken)
        {
            PersonalAccessToken token =
                await _resourceManager.GetExistingOrCreateNewAsync(request.Id, cancellationToken);

            await ValidateTenantAccess(token, cancellationToken);

            token.UserName = request.UserName ?? token.UserName;
            token.AllowedScopes = request.AllowedScopes ?? token.AllowedScopes;
            token.AllowedApplicationIds =
                request.AllowedApplicationIds ?? token.AllowedApplicationIds;
            token.ClaimsExtensions = request.ClaimsExtensions ?? token.ClaimsExtensions;
            token.Source = request.Source ?? token.Source;

            SaveResourceResult<PersonalAccessToken> result = await _resourceManager
                .SaveAsync(token, cancellationToken);

            return result.Resource;
        }

        public override bool IsAllowedToPublish()
        {
            return UserContext.HasPermission(
                Permissions.ClientAuthoring.PersonalAccessToken.Publish);
        }

        public override bool IsAllowedToApprove()
        {
            return UserContext.HasPermission(Permissions.ClientAuthoring.Approve);
        }
    }
}
