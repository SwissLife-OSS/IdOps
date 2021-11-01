using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.IdentityServer>;
using Group = MongoDB.Driver.Builders<IdOps.Model.IdentityServerGroup>;

namespace IdOps.Server.Storage.Mongo
{
    public class IdentityServerStore : ResourceStore<Model.IdentityServer>, IIdentityServerStore
    {
        private readonly IIdOpsDbContext _dbContext;

        public IdentityServerStore(IIdOpsDbContext dbContext) : base(dbContext.IdentityServers)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Model.IdentityServer>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.IdentityServers
                .Find(FilterDefinition<Model.IdentityServer>.Empty)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<IdentityServerGroup>> GetAllGroupsAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.IdentityServerGroups.AsQueryable()
                .ToListAsync(cancellationToken);
        }

        public async Task<IdentityServerGroup> GetGroupByTenantAsync(
            string? tenant,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter =
                FilterDefinition<IdentityServerGroup>.Empty;

            if (tenant is not null)
            {
                filter &= Group.Filter.Eq($"{nameof(IdentityServerGroup.Tenants)}", tenant);
            }

            return await _dbContext.IdentityServerGroups
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IdentityServerGroup> GetGroupByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdentityServerGroup> filter =
                Group.Filter.Eq(x => x.Id, id);

            return await _dbContext.IdentityServerGroups
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public override async Task<IReadOnlyList<Model.IdentityServer>> GetResourceWithOpenApproval(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Model.IdentityServer> filter = Filter.Empty;

            if (ids != null)
            {
                filter &= Filter.In(x => x.Id, ids);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public override async Task<IReadOnlyList<Model.IdentityServer>> GetAllAsync(
            IEnumerable<Guid>? ids,
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Model.IdentityServer> filter =
                FilterDefinition<Model.IdentityServer>.Empty;

            if (ids != null)
            {
                filter &= Filter.In(x => x.Id, ids);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
