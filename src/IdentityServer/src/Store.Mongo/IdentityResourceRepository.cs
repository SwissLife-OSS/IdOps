using System.Collections.Generic;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using IdOps.IdentityServer.Model;

namespace  IdOps.IdentityServer.Store.Mongo
{
    public class IdentityResourceRepository : IIdentityResourceRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public IdentityResourceRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IdOpsIdentityResource>> GetByNameAsync(
            IEnumerable<string> names,
            CancellationToken cancellationToken)
        {
            List<IdOpsIdentityResource>? resources = await _dbContext
                .IdentityResources.AsQueryable()
                    .Where(x => names.Contains(x.Name))
                    .ToListAsync(cancellationToken);

            return resources;
        }

        public async Task<IEnumerable<IdOpsIdentityResource>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _dbContext.IdentityResources.AsQueryable()
                .ToListAsync(cancellationToken);
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsIdentityResource identityResource,
            CancellationToken cancellationToken)
        {
            var updater = new ResourceUpdater<IdOpsIdentityResource>(_dbContext.IdentityResources);

            return await updater.UpdateAsync(identityResource, cancellationToken);
        }

        private async Task<IdOpsIdentityResource?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            IdOpsIdentityResource resource = await _dbContext.IdentityResources
                .AsQueryable()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync(cancellationToken);

            return resource;
        }
    }
}

