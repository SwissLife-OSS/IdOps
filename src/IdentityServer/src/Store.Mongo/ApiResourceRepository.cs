using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace  IdOps.IdentityServer.Storage.Mongo
{
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public ApiResourceRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IdOpsApiResource>> GetByNameAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken)
        {
            List<IdOpsApiResource>? resources = await _dbContext.ApiResources
                .AsQueryable()
                .Where(x => names.Contains(x.Name))
                .ToListAsync(cancellationToken);

            return resources;
        }

        public async Task<IEnumerable<IdOpsApiResource>> GetByScopeNameAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken)
        {
            List<IdOpsApiResource>? resources = await _dbContext.ApiResources
                .AsQueryable()
                .Where(api => api.Scopes.Any(x => scopeNames.Contains(x)))
                .ToListAsync(cancellationToken);

            return resources;
        }

        public async Task<IEnumerable<IdOpsApiResource>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            List<IdOpsApiResource>? resources = await _dbContext.ApiResources
                .AsQueryable()
                .ToListAsync(cancellationToken);

            return resources;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsApiResource apiResource,
            CancellationToken cancellationToken)
        {
            var updater = new ResourceUpdater<IdOpsApiResource>(_dbContext.ApiResources);

            return await updater.UpdateAsync(apiResource, cancellationToken);
        }

        private async Task<IdOpsApiResource?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            return await _dbContext.ApiResources
                .AsQueryable()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

}
