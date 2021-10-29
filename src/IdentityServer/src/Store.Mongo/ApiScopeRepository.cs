using System.Collections.Generic;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Duende.IdentityServer.Models;
using IdOps.IdentityServer.Model;

namespace  IdOps.IdentityServer.Store.Mongo
{
    public class ApiScopeRepository : IApiScopeRepository
    {
        private readonly IIdentityStoreDbContext _dbContext;

        public ApiScopeRepository(IIdentityStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ApiScope>> GetByNameAsync(
            IEnumerable<string> scopeNames,
            CancellationToken cancellationToken)
        {
            List<IdOpsApiScope>? scopes = await _dbContext.ApiScopes
                .AsQueryable()
                .Where(x => scopeNames.Contains(x.Name))
                .ToListAsync(cancellationToken);

            return scopes;
        }

        public async Task<IEnumerable<ApiScope>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            List<IdOpsApiScope>? scopes = await _dbContext.ApiScopes
                .AsQueryable()
                .ToListAsync(cancellationToken);

            return scopes;
        }

        public async Task<UpdateResourceResult> UpdateAsync(
            IdOpsApiScope apiScope,
            CancellationToken cancellationToken)
        {
            var updater = new ResourceUpdater<IdOpsApiScope>(_dbContext.ApiScopes);

            return await updater.UpdateAsync(apiScope, cancellationToken);
        }


        private async Task<IdOpsApiScope?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            return await _dbContext.ApiScopes
                .AsQueryable()
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}

