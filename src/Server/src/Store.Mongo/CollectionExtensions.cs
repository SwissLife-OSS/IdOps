using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdOps.Store.Mongo
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

            long totalCount = await cursor.CountDocumentsAsync(cancellationToken);

            List<T> items = await cursor
                .Skip(request.PageNr * request.PageSize)
                .Limit(request.PageSize)
                .ToListAsync(cancellationToken);

            return new SearchResult<T>(items, totalCount > request.PageSize)
            {
                TotalCount = (int)totalCount
            };
        }
    }
}
