using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IApprovalService
    {
        Task<IEnumerable<ResourceApprovalLog>> GetResourceApprovalLog(
            IReadOnlyList<Guid> ids,
            CancellationToken cancellationToken);

        IAsyncEnumerable<ResourceApproval> GetResourceApprovals(
            ResourceApprovalRequest? filter,
            CancellationToken cancellationToken);

        Task<ApproveResourcesResult> ApproveResourcesAsync(
            ApproveResourcesRequest request,
            CancellationToken cancellationToken);
    }
}
