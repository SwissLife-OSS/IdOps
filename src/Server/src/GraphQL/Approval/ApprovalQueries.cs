using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
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
            IReadOnlyList<IFieldSelection>? selections = context
                .GetSelections((ObjectType)context.ObjectType,
                    context.Selection.SyntaxNode.SelectionSet);

            if (selections is { } s && s.Count() == 1 && s[0].Field.Name == "id")
            {
                return new Model.Environment { Id = environment.EnvironmentId };
            }

            return await environmentById.LoadAsync(environment.EnvironmentId, cancellationToken);
        }
    }
}
