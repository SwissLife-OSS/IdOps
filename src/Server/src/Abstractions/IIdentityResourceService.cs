using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IIdentityResourceService : IResourceService<IdentityResource>
    {
        Task<IdentityResource> SaveAsync(
            SaveIdentityResourceRequest request,
            CancellationToken cancellationToken);
    }
}
