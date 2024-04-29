using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL.Publish
{
    [ExtendObjectType(RootTypes.Query)]
    public class ApprovalQueries
    {
        private readonly IApprovalService _approvalService;

        public ApprovalQueries(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceApproval)]
        public IAsyncEnumerable<ResourceApproval> GetResourceApprovals(
            ResourceApprovalRequest? input,
            CancellationToken cancellationToken)
        {
            return _approvalService
                .GetResourceApprovals(input, cancellationToken);
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public Task<IEnumerable<ResourceApprovalLog>> GetResourceApprovalLog(
            ResourceApprovalLogRequest input,
            CancellationToken cancellationToken)
        {
            return _approvalService
                .GetResourceApprovalLog(new[] { input.ResourceId }, cancellationToken);
        }
    }

    public class ResourceApprovalEnvironmentType : ObjectType<ResourceApprovalEnvironment>
    {
        protected override void Configure(
            IObjectTypeDescriptor<ResourceApprovalEnvironment> descriptor)
        {
            descriptor.Ignore(x => x.EnvironmentId);

            descriptor.Field("environment")
                .ResolveWith<ResourceApprovalResolvers>(_ =>
                    _.GetEnvironmentAsync(default!, default!, default!, default!));
        }
    }

    internal class ResourceApprovalResolvers
    {
        public async Task<Model.Environment> GetEnvironmentAsync(
            [Parent] ResourceApprovalEnvironment environment,
            EnvironmentByIdDataLoader environmentById,
            IResolverContext context,
            CancellationToken cancellationToken)
        {
            var selections = context.GetSelections((ObjectType)context.Selection.Type.NamedType());

            if (selections is { Count: 1 } && selections[0].Field.Name is "id")
            {
                return new Model.Environment { Id = environment.EnvironmentId };
            }

            return await environmentById.LoadAsync(environment.EnvironmentId, cancellationToken);
        }
    }
}
