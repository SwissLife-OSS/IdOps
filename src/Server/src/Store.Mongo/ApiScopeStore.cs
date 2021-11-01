using IdOps.Model;

namespace IdOps.Server.Storage.Mongo
{
    public class ApiScopeStore : TenantResourceStore<ApiScope>, IApiScopeStore
    {
        public ApiScopeStore(IIdOpsDbContext dbContext) : base(dbContext.ApiScopes)
        {
        }
    }
}
