using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using static MongoDB.Driver.Builders<IdOps.Model.ClientTemplate>;

namespace IdOps.Server.Storage.Mongo
{
    public class ClientTemplateStore : IClientTemplateStore
    {
        public ClientTemplateStore(IIdOpsDbContext dbContext)
        {
            Collection = dbContext.ClientTemplates;
        }

        protected IMongoCollection<ClientTemplate> Collection { get; }

        public async Task<ClientTemplate?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ClientTemplate> filter = Filter.Eq(x => x.Id, id);

            return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<ClientTemplate>> GetManyAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ClientTemplate> filter = Filter.In(x => x.Id, ids);

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ClientTemplate>> GetAllAsync(
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ClientTemplate> filter = Filter.Empty;

            if (tenants != null)
            {
                filter = Filter.In(x => x.Tenant, tenants);
            }

            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task SaveClientTemplate(
            ClientTemplate clientTemplate,
            CancellationToken cancellationToken)
        {
            FilterDefinition<ClientTemplate> filter = Filter.Eq(x => x.Id, clientTemplate.Id);

            ReplaceOptions options = new() { IsUpsert = true };

            await Collection.ReplaceOneAsync(filter, clientTemplate, options, cancellationToken);
        }
    }
}
