using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.IdentityServerGroup>;

namespace IdOps.Server.Storage.Mongo
{
    public class IdentityServerGroupStore : IIdentityServerGroupStore
    {
        public IdentityServerGroupStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.IdentityServerGroups;
        }

        protected IMongoCollection<IdentityServerGroup> Collection { get; }

        public async Task<IReadOnlyList<IdentityServerGroup>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter = FilterDefinition<IdentityServerGroup>.Empty;

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IdentityServerGroup> SaveAsync(
            IdentityServerGroup identityServerGroup,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter = Filter.Eq(x => x.Id, identityServerGroup.Id);

            ReplaceOptions options = new() { IsUpsert = true };

            await Collection.ReplaceOneAsync(filter, identityServerGroup, options, cancellationToken);

            return identityServerGroup;
        }

        public async Task<IdentityServerGroup?> GetGroupByTenantAsync(
            string? tenant,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter =
                FilterDefinition<IdentityServerGroup>.Empty;

            if (tenant is not null)
            {
                filter &= Filter.Eq($"{nameof(IdentityServerGroup.Tenants)}", tenant);
            }

            return await Collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IdentityServerGroup?> GetGroupByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter =
                Filter.Eq(x => x.Id, id);

            return await Collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
