using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class ClientRepository : IClientRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public ClientRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IdOpsClient?> GetAsync(
            string clientId,
            CancellationToken cancellationToken)
        {
            FilterDefinition<IdOpsClient> findFilter =
                Builders<IdOpsClient>.Filter.Eq(client => client.ClientId, clientId);

            return (await _dbContext.Clients
                    .FindAsync(findFilter, cancellationToken: cancellationToken)
                    .ConfigureAwait(false))
                .ToList()
                .SingleOrDefault();
        }

        public async Task<HashSet<string>> GetAllClientOrigins()
        {
            BsonDocument allOriginDocument = await _dbContext.Clients
                .Aggregate()
                .Project(c => new { Origin = c.AllowedCorsOrigins })
                .Unwind<BsonDocument>("Origin")
                .Group(new BsonDocument
                {
                    { "_id", new BsonDocument{{"Key", "unique" }}},
                    { "all", new BsonDocument {{ "$addToSet", "$Origin" }}}
                })
                .SingleOrDefaultAsync();

            if (allOriginDocument == null)
            {
                return new HashSet<string>();
            }

            IEnumerable<string> allOrigins = allOriginDocument
                .GetValue("all")
                .AsBsonArray
                .Select(b => b.AsString);

            var allClientOrigins = new HashSet<string>(
                allOrigins,
                StringComparer.InvariantCultureIgnoreCase);

            return allClientOrigins;
        }

        public async Task<HashSet<string>> GetAllClientRedirectUriAsync()
        {
            BsonDocument allUriDocument = await _dbContext.Clients
                .Aggregate()
                .Project(c => new { Uri = c.RedirectUris })
                .Unwind<BsonDocument>("Uri")
                .Group(new BsonDocument
                {
                    { "_id", new BsonDocument{{"Key", "unique" }}},
                    { "all", new BsonDocument {{ "$addToSet", "$Uri" } }}
                })
                .SingleOrDefaultAsync();

            if (allUriDocument == null)
            {
                return new HashSet<string>();
            }

            IEnumerable<string> allUris = allUriDocument
                .GetValue("all")
                .AsBsonArray
                .Select(b => b.AsString);

            var allClientRedirectUris = new HashSet<string>(
                allUris,
                StringComparer.InvariantCultureIgnoreCase);

            return allClientRedirectUris;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsClient client,
            CancellationToken cancellationToken)
        {
            var updater = new ResourceUpdater<IdOpsClient>(_dbContext.Clients);

            return await updater.UpdateAsync(client, cancellationToken);
        }
    }
}
