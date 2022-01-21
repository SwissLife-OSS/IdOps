using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdOps.Server.Storage.Mongo
{
    public abstract class AllowedScopesResourceStore<T> : ResourceStore<T>//, ITenantResourceStore<T>
        where T : class, IAllowedScopesResource, new()
    {
        protected AllowedScopesResourceStore(IMongoCollection<T> collection) : base(collection)
        {
        }

        public async Task<IReadOnlyList<T>> GetByAllowedScopesAsync(
            IEnumerable<Guid> apiScopeIds,
            CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter = new FilterDefinitionBuilder<T>().ElemMatch(
                field: c => c.GetAllowedScopesIds(),
                filter: p => apiScopeIds.Contains(p));

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
