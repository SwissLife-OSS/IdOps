using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.PersonalAccessToken>;

namespace IdOps.Server.Storage.Mongo
{
    public class PersonalAccessTokenStore
        : TenantResourceStore<PersonalAccessToken>, IPersonalAccessTokenStore
    {
        public PersonalAccessTokenStore(IIdOpsDbContext context)
            : base(context.PersonalAccessTokens)
        {
        }

        public async Task<PersonalAccessToken> CreateAsync(
            PersonalAccessToken token,
            CancellationToken cancellationToken)
        {
            await Collection
                .InsertOneAsync(token, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return token;
        }

        public async Task<SearchResult<PersonalAccessToken>> SearchAsync(
            SearchPersonalAccessTokensRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersonalAccessToken> filter = Filter.Empty;

            if (request.Tenants != null)
            {
                filter &= Filter.Where(x => request.Tenants.Contains(x.Tenant));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                filter &= Filter.Regex(
                    x => x.Title,
                    new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*", "i"));
            }

            if (request.EnvironmentId.HasValue)
            {
                filter &= Filter.Eq(nameof(PersonalAccessToken.EnvironmentId),
                    request.EnvironmentId);
            }

            return await Collection.ExecuteSearchAsync(filter, request, cancellationToken);
        }

        public async Task<IReadOnlyList<PersonalAccessToken>> GetByAllowedScopesAsync(
            Guid scope,
            CancellationToken cancellationToken)
        {
            FilterDefinition<PersonalAccessToken> filter =
                Filter.Where(p => p.AllowedScopes.Contains(scope));

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
