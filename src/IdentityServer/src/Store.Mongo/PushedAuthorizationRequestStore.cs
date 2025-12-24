using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using MongoDB.Driver;

namespace IdOps.IdentityServer.Storage.Mongo
{
    /// <summary>
    /// MongoDB implementation of <see cref="IPushedAuthorizationRequestStore"/>.
    /// </summary>
    public class PushedAuthorizationRequestStore : IPushedAuthorizationRequestStore
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public PushedAuthorizationRequestStore(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task StoreAsync(PushedAuthorizationRequest pushedAuthorizationRequest)
        {
            await _dbContext.PushedAuthorizationRequests
                .ReplaceOneAsync(
                    x => x.ReferenceValueHash == pushedAuthorizationRequest.ReferenceValueHash,
                    pushedAuthorizationRequest,
                    options: new ReplaceOptions { IsUpsert = true })
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<PushedAuthorizationRequest?> GetByHashAsync(string referenceValueHash)
        {
            FilterDefinition<PushedAuthorizationRequest> filter =
                Builders<PushedAuthorizationRequest>.Filter.Eq(
                    x => x.ReferenceValueHash,
                    referenceValueHash);

            return (await _dbContext.PushedAuthorizationRequests
                    .FindAsync(filter)
                    .ConfigureAwait(false))
                .ToList()
                .SingleOrDefault();
        }

        /// <inheritdoc/>
        public async Task ConsumeByHashAsync(string referenceValueHash)
        {
            FilterDefinition<PushedAuthorizationRequest> filter =
                Builders<PushedAuthorizationRequest>.Filter.Eq(
                    x => x.ReferenceValueHash,
                    referenceValueHash);

            await _dbContext.PushedAuthorizationRequests
                .DeleteOneAsync(filter)
                .ConfigureAwait(false);
        }
    }
}
