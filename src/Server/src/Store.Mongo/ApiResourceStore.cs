using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.ApiResource>;

namespace IdOps.Server.Storage.Mongo
{
    public class ApiResourceStore : TenantResourceStore<ApiResource>, IApiResourceStore
    {
        public ApiResourceStore(IIdOpsDbContext dbContext) : base(dbContext.ApiResources)
        {
        }

        public async Task<IReadOnlyList<ApiResource>> GetByScopesAsync(
            Guid scope,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ApiResource> filter =
                Filter.Where(p => p.Scopes.Contains(scope));

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
