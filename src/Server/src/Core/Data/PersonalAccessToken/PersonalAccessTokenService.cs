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
        private readonly IResourceManager _resourceManager;
        private readonly IPersonalAccessTokenStore _store;

        public PersonalAccessTokenService(
            IdOpsServerOptions options,
            IPersonalAccessTokenStore store,
            IHashAlgorithmResolver resolver,
            IPasswordProvider passwordProvider,
            IResourceManager resourceManager,
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

        public async Task<AddSecretPersonalAccessTokenResult> AddSecretToTokenAsync(
            DateTime expiresAt,
            Guid id,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<PersonalAccessToken> context = await _resourceManager
                .GetExistingOrCreateNewAsync<PersonalAccessToken>(id, cancellationToken);

            await ValidateTenantAccess(context.Resource, cancellationToken);

            if (expiresAt <= DateTime.UtcNow || expiresAt > DateTime.UtcNow.AddYears(2).AddDays(1))
            {
                ErrorException.Throw(new ExpiresAtInvalid(expiresAt));
            }

            if (!_resolver.TryResolve(context.Resource.HashAlgorithm, out IHashAlgorithm? encryptor))
            {
                ErrorException.Throw(new HashAlgorithmNotFound(context.Resource.HashAlgorithm));
            }

            string secret = _passwordProvider.GenerateRandomPassword(64);
            string hash = encryptor.ComputeHash(secret);
            context.Resource.Tokens.Add(new(Guid.NewGuid(), hash, expiresAt, DateTime.UtcNow));

            SaveResourceResult<PersonalAccessToken> result =
                await _resourceManager.SaveAsync(context, cancellationToken);

            return new AddSecretPersonalAccessTokenResult(result.Resource, secret);
        }

        public async Task<RemoveSecretPersonalAccessTokenResult> RemoveSecretOfTokenAsync(
            Guid id,
            Guid tokenId,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<PersonalAccessToken> context = await _resourceManager
                .GetExistingOrCreateNewAsync<PersonalAccessToken>(id, cancellationToken);

            await ValidateTenantAccess(context.Resource, cancellationToken);

            HashedToken? token = context.Resource.Tokens.FirstOrDefault(x => x.Id == tokenId);
            if (token is null)
            {
                return new RemoveSecretPersonalAccessTokenResult(null, null);
            }

            context.Resource.Tokens.Remove(token);

            SaveResourceResult<PersonalAccessToken> result =
                await _resourceManager.SaveAsync(context, cancellationToken);

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
                .SetAllowedScopes(request.AllowedScopes)
                .SetHashAlgorithm(request.HashAlgorithm);

            foreach (var (type, value) in request.ClaimsExtensions)
            {
                builder.AddClaimExtension(type, value);
            }

            PersonalAccessToken token = builder.Build();

            await ValidateTenantAccess(token, cancellationToken);

            ResourceChangeContext<PersonalAccessToken> context = _resourceManager
                .SetNewVersion(token);

            SaveResourceResult<PersonalAccessToken> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

            return new CreatePersonalAccessTokenResult(result.Resource);
        }

        public async Task<PersonalAccessToken> UpdateAsync(
            UpdatePersonalAccessTokenRequest request,
            CancellationToken cancellationToken)
        {
            ResourceChangeContext<PersonalAccessToken> context = await _resourceManager
                .GetExistingOrCreateNewAsync<PersonalAccessToken>(request.Id, cancellationToken);

            await ValidateTenantAccess(context.Resource, cancellationToken);

            context.Resource.UserName = request.UserName ?? context.Resource.UserName;
            context.Resource.AllowedScopes = request.AllowedScopes ?? context.Resource.AllowedScopes;
            context.Resource.AllowedApplicationIds = request.AllowedApplicationIds ?? context.Resource.AllowedApplicationIds;
            context.Resource.ClaimsExtensions = request.ClaimsExtensions ?? context.Resource.ClaimsExtensions;
            context.Resource.Source = request.Source ?? context.Resource.Source;

            SaveResourceResult<PersonalAccessToken> result = await _resourceManager
                .SaveAsync(context, cancellationToken);

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
