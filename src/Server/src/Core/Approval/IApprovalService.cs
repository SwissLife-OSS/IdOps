using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps
{
    public interface IApprovalService
    {
        IAsyncEnumerable<ResourceApproval> GetResourceApprovals(
            ResourceApprovalRequest? filter,
            CancellationToken cancellationToken);

        Task<ApproveResourcesResult> ApproveResourcesAsync(
            ApproveResourcesRequest request,
            CancellationToken cancellationToken);
    }
}
