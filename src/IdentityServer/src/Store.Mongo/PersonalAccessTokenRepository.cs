using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.IdOpsPersonalAccessToken>;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class PersonalAccessTokenRepository : IPersonalAccessTokenRepository
    {
        private static readonly InsertOneOptions _insertOneOptions = new();
        private readonly IIdentityStoreDbContext _context;

        public PersonalAccessTokenRepository(IIdentityStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<IdOpsPersonalAccessToken>> GetActiveTokensByUserNameAsync(
            string userName,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdOpsPersonalAccessToken>? filter =
                Filter.Eq(x => x.UserName, userName) &
                Filter.Gt(WellKnownPatFields.ExpiresAt, DateTime.UtcNow) &
                Filter.Eq(WellKnownPatFields.IsUsed, false);

            return await _context.PersonalAccessTokens
                .Find(filter)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IdOpsPersonalAccessToken?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdOpsPersonalAccessToken>? filter = Filter.Eq(x => x.Id, id);

            return await _context.PersonalAccessTokens
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IdOpsPersonalAccessToken> CreateAsync(
            IdOpsPersonalAccessToken pat,
            CancellationToken cancellationToken)
        {
            await _context.PersonalAccessTokens
                .InsertOneAsync(pat, _insertOneOptions, cancellationToken)
                .ConfigureAwait(false);

            return pat;
        }

        public async Task<IdOpsPersonalAccessToken> SaveAsync(
            IdOpsPersonalAccessToken pat,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdOpsPersonalAccessToken>? filter =
                Filter.Eq(p => p.Id, pat.Id);

            ReplaceOptions options = new() { IsUpsert = false };

            await _context.PersonalAccessTokens
                .ReplaceOneAsync(filter, pat, options, cancellationToken)
                .ConfigureAwait(false);

            return pat;
        }

        public async Task<UpdateResourceResult> UpdateResourceAsync(
            IdOpsPersonalAccessToken apiResource,
            CancellationToken cancellationToken)
        {
            var updater =
                new ResourceUpdater<IdOpsPersonalAccessToken>(_context.PersonalAccessTokens);

            return await updater.UpdateAsync(apiResource, cancellationToken);
        }
    }
}
