using IdOps.Model;

namespace IdOps.Store.Mongo
{
    public class ApiScopeStore : TenantResourceStore<ApiScope>, IApiScopeStore
    {
        public ApiScopeStore(IIdOpsDbContext dbContext) : base(dbContext.ApiScopes)
        {
        }
    }
}
