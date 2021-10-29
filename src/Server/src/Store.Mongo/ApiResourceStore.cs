using IdOps.Model;

namespace IdOps.Store.Mongo
{
    public class ApiResourceStore : TenantResourceStore<ApiResource>, IApiResourceStore
    {
        public ApiResourceStore(IIdOpsDbContext dbContext) : base(dbContext.ApiResources)
        {
        }
    }
}
