using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using static MongoDB.Driver.Builders<IdOps.Model.Client>;

namespace IdOps.Server.Storage.Mongo
{
    public class ClientStore : TenantResourceStore<Client>, IClientStore
    {
        public ClientStore(IIdOpsDbContext dbContext) : base(dbContext.Clients)
        {
        }

        public Task<SearchResult<Client>> SearchAsync(
            SearchClientsRequest request,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Client> filter = Filter.Empty;

            if (request.Tenants is not null)
            {
                filter &= Filter.Where(x => request.Tenants.Contains(x.Tenant));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                BsonRegularExpression regex = new($".*{Regex.Escape(request.SearchText)}.*", "i");
                filter &= Filter.Regex(x => x.Name, regex);
            }

            if (request.EnvironmentId.HasValue)
            {
                filter &= Filter.Eq(nameof(Client.Environments), request.EnvironmentId);
            }

            return Collection.ExecuteSearchAsync(filter, request, cancellationToken);
        }

        public async Task<IReadOnlyList<Client>> GetByAllowedScopesAsync(
            Guid apiScopeId,
            CancellationToken cancellationToken)
        {
            FilterDefinition<Client> filter =
                Filter.ElemMatch(
                    field: c => c.AllowedScopes,
                    filter: p => p.Id == apiScopeId);

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
