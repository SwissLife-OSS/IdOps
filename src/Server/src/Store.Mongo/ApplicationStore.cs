using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.Application>;

namespace IdOps.Server.Storage.Mongo
{
    public class ApplicationStore : TenantResourceStore<Application>, IApplicationStore
    {
        public ApplicationStore(IIdOpsDbContext dbContext) : base(dbContext.Applications)
        {
        }

        public Task<SearchResult<Application>> SearchAsync(
            SearchApplicationsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Application> filter = Filter.Empty;

            if (request.Tenants != null)
            {
                filter &= Filter.Where(x => request.Tenants.Contains(x.Tenant));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                filter &= Filter.Regex(
                    x => x.Name,
                    new BsonRegularExpression($".*{Regex.Escape(request.SearchText)}.*", "i"));
            }

            return Collection.ExecuteSearchAsync(filter, request, cancellationToken);
        }

        public async Task<Application?> GetByClientIdAsync(
            Guid clientId,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Application> filter =
                Filter.Eq($"{nameof(Application.ClientIds)}", clientId);

            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
