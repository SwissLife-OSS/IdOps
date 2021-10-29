using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.DataConnector;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdOps.IdentityServer.Store.Mongo
{
    public class UserDataConnectorDataRepository : IUserDataConnectorDataRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public UserDataConnectorDataRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDataConnectorData> GetAsync(
            string key,
            string connector,
            CancellationToken cancellationToken)
        {
            return await _dbContext.ConnectorData.AsQueryable()
                .Where(x => x.Key == key && x.Connector == connector)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveAsync(
            UserDataConnectorData data,
            CancellationToken cancellationToken)
        {
            FilterDefinition<UserDataConnectorData> filter =
                Builders<UserDataConnectorData>.Filter
                    .Eq(x => x.Key, data.Key) &
                 Builders<UserDataConnectorData>.Filter
                    .Eq(x => x.Connector, data.Connector);

            await _dbContext.ConnectorData.ReplaceOneAsync(
                filter,
                data,
                options: new ReplaceOptions { IsUpsert = true },
                cancellationToken);
        }
    }
}
