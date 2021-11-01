using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Server.Storage
{
    public interface IResouceAuditStore
    {
        Task CreateAsync(ResourceAuditEvent audit, CancellationToken cancellationToken);
        Task<SearchResult<ResourceAuditEvent>> SearchAsync(SearchResourceAuditRequest request, CancellationToken cancellationToken);
    }
}
