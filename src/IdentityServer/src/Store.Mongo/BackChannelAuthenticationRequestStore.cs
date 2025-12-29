using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using IdOps.IdentityServer.Storage.Mongo.Model;
using MongoDB.Driver;

namespace IdOps.IdentityServer.Storage.Mongo
{
    /// <summary>
    /// MongoDB implementation of <see cref="IBackChannelAuthenticationRequestStore"/>.
    /// </summary>
    public class BackChannelAuthenticationRequestStore : IBackChannelAuthenticationRequestStore
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public BackChannelAuthenticationRequestStore(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<string> CreateRequestAsync(BackChannelAuthenticationRequest request)
        {
            var serialized = SerializedBackChannelAuthenticationRequest
                .FromBackChannelAuthenticationRequest(request);

            if (string.IsNullOrEmpty(serialized.InternalId))
            {
                serialized.InternalId = Guid.NewGuid().ToString();
            }

            await _dbContext.BackChannelAuthenticationRequests
                .InsertOneAsync(serialized)
                .ConfigureAwait(false);

            return serialized.InternalId;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BackChannelAuthenticationRequest>> GetLoginsForUserAsync(
            string subjectId,
            string? clientId = null)
        {
            var filterBuilder = Builders<SerializedBackChannelAuthenticationRequest>.Filter;
            
            // Filter by subject claim with type "sub"
            var filter = filterBuilder.ElemMatch(
                x => x.SubjectClaims,
                c => c.Type == "sub" && c.Value == subjectId);

            if (!string.IsNullOrEmpty(clientId))
            {
                filter = filter & filterBuilder.Eq(x => x.ClientId, clientId);
            }

            var results = await _dbContext.BackChannelAuthenticationRequests
                .Find(filter)
                .ToListAsync()
                .ConfigureAwait(false);

            return results.Select(r => r.ToBackChannelAuthenticationRequest());
        }

        /// <inheritdoc/>
        public async Task<BackChannelAuthenticationRequest?> GetByAuthenticationRequestIdAsync(
            string requestId)
        {
            var filter = Builders<SerializedBackChannelAuthenticationRequest>.Filter
                .Eq(x => x.InternalId, requestId);

            var result = await _dbContext.BackChannelAuthenticationRequests
                .Find(filter)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return result?.ToBackChannelAuthenticationRequest();
        }

        /// <inheritdoc/>
        public async Task<BackChannelAuthenticationRequest?> GetByInternalIdAsync(string id)
        {
            var filter = Builders<SerializedBackChannelAuthenticationRequest>.Filter
                .Eq(x => x.InternalId, id);

            var result = await _dbContext.BackChannelAuthenticationRequests
                .Find(filter)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return result?.ToBackChannelAuthenticationRequest();
        }

        /// <inheritdoc/>
        public async Task RemoveByInternalIdAsync(string id)
        {
            var filter = Builders<SerializedBackChannelAuthenticationRequest>.Filter
                .Eq(x => x.InternalId, id);

            await _dbContext.BackChannelAuthenticationRequests
                .DeleteOneAsync(filter)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task UpdateByInternalIdAsync(
            string id,
            BackChannelAuthenticationRequest request)
        {
            var serialized = SerializedBackChannelAuthenticationRequest
                .FromBackChannelAuthenticationRequest(request);

            serialized.InternalId = id;

            await _dbContext.BackChannelAuthenticationRequests
                .ReplaceOneAsync(
                    x => x.InternalId == id,
                    serialized,
                    new ReplaceOptions { IsUpsert = false })
                .ConfigureAwait(false);
        }
    }
}
