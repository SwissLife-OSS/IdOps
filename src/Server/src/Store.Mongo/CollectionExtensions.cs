using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdOps.Server.Storage.Mongo
{
    public static class CollectionExtensions
    {
        public static async Task<SearchResult<T>> ExecuteSearchAsync<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            PagedRequest request,
            CancellationToken cancellationToken)
        {
            return await collection
                .ExecuteSearchAsync(filter, request, null, cancellationToken);
        }

        public static async Task<SearchResult<T>> ExecuteSearchAsync<T>(
            this IMongoCollection<T> collection,
            FilterDefinition<T> filter,
            PagedRequest request,
            SortDefinition<T>? sort,
            CancellationToken cancellationToken)
        {
            IFindFluent<T, T>? cursor = collection.Find(filter);

            if (sort is not null)
            {
                cursor = cursor.Sort(sort);
            }

            List<T> items = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize + 1)
                .ToListAsync(cancellationToken);

            var hasMore = items.Count > request.PageSize;

            return new SearchResult<T>(items, hasMore);
        }
    }
}
