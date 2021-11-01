using IdOps.Model;

namespace IdOps.Server.Storage.Mongo
{
    public class ApiResourceStore : TenantResourceStore<ApiResource>, IApiResourceStore
    {
        public ApiResourceStore(IIdOpsDbContext dbContext) : base(dbContext.ApiResources)
        {
        }
    }
}
