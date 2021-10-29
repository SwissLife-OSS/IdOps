using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;

namespace IdOps.GraphQL.Publish
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class ApprovalMutations
    {
        private readonly IApprovalService _approvalService;

        public ApprovalMutations(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceApproval)]
        public Task<ApproveResourcesResult> ApproveResources(
            ApproveResourcesRequest input,
            CancellationToken cancellationToken)
        {
            return _approvalService.ApproveResourcesAsync(input, cancellationToken);
        }
    }
}
