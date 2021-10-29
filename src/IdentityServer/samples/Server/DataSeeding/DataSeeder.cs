using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Store.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace IdOps.IdentityServer.Samples.DataSeeding
{
    public class DataSeeder
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public DataSeeder(IIdentityStoreDbContext identityDbContext)
        {
            _dbContext = identityDbContext;
        }

        public async Task SeedSampleDataAsync(CancellationToken cancellationToken)
        {
            var resCount = await _dbContext.IdentityResources.AsQueryable()
                .CountAsync();

            var clientCount = await _dbContext.Clients.AsQueryable()
                .CountAsync();

            var apiResCount = await _dbContext.ApiResources.AsQueryable()
                .CountAsync();

            var apiScopeCount = await _dbContext.ApiScopes.AsQueryable()
                .CountAsync();

            if (resCount == 0)
            {
                await CreateIdentityResourcesAsync(SampleData.IdentityResources, cancellationToken);
            }

            if (clientCount == 0)
            {
                await CreateClientsAsync(SampleData.Clients, cancellationToken);
            }

            if (apiResCount == 0)
            {
                await CreateApiResources(SampleData.ApiResources, cancellationToken);
            }

            if (apiScopeCount == 0)
            {
                await CreateApiScopes(SampleData.ApiScopes, cancellationToken);
            }
        }

        public async Task CreateIdentityResourcesAsync(
            IEnumerable<IdOpsIdentityResource> resources,
            CancellationToken cancellationToken)
        {

            await _dbContext.IdentityResources.InsertManyAsync(
                resources,
                options: null,
                cancellationToken);
        }

        public async Task CreateClientsAsync(
            IEnumerable<IdOpsClient> clients,
            CancellationToken cancellationToken)
        {
            await _dbContext.Clients.InsertManyAsync(
                clients,
                options: null,
                cancellationToken);
        }

        public async Task CreateApiResources(
            IEnumerable<IdOpsApiResource> resources,
            CancellationToken cancellationToken)
        {
            await _dbContext.ApiResources.InsertManyAsync(
                resources,
                options: null,
                cancellationToken);
        }

        public async Task CreateApiScopes(
            IEnumerable<IdOpsApiScope> scopes,
            CancellationToken cancellationToken)
        {
            await _dbContext.ApiScopes.InsertManyAsync(
                scopes,
                options: null,
                cancellationToken);
        }
    }
}
