using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.GraphQL.DataLoaders;
using IdOps.Model;

namespace IdOps.GraphQL.Publish
{
    [ExtendObjectType(RootTypes.Query)]
    public class PublishQueries
    {
        private readonly IPublishingService _publishingService;

        public PublishQueries(IPublishingService publishingService)
        {
            _publishingService = publishingService;
        }

        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceAuthoringRead)]
        public IAsyncEnumerable<PublishedResource> GetPublishedResoucesAsync(
            PublishedResourcesRequest? input,
            CancellationToken cancellationToken) =>
            _publishingService
                .GetPublishedResourcesAsync(input, cancellationToken);

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public Task<IEnumerable<ResourcePublishLog>> GetResourcePublishingLog(
            ResourcePublishingLogRequest input,
            CancellationToken cancellationToken)
        {
            return _publishingService
                .GetResourcePublishingLog(new[] { input.ResourceId }, cancellationToken);
        }
    }

    public class PublishedResourceEnvironmentType : ObjectType<PublishedResourceEnvironment>
    {
        protected override void Configure(
            IObjectTypeDescriptor<PublishedResourceEnvironment> descriptor)
        {
            descriptor.Ignore(x => x.EnvironmentId);

            descriptor.Field("environment")
                .ResolveWith<PublishedResourceResolvers>(_ =>
                    _.GetEnvironmentAsync(default!, default!, default!, default!));
        }
    }

    internal class PublishedResourceResolvers
    {
        public async Task<Model.Environment> GetEnvironmentAsync(
            [Parent] PublishedResourceEnvironment environment,
            EnvironmentByIdDataLoader environmentById,
            IResolverContext context,
            CancellationToken cancellationToken)
        {
            var selections = context.GetSelections((ObjectType)context.Selection.Type.NamedType());

            if (selections is { } s && s.Count() == 1 && s[0].Field.Name == "id")
            {
                return new Model.Environment { Id = environment.EnvironmentId };
            }

            return await environmentById.LoadAsync(environment.EnvironmentId, cancellationToken);
        }

        //public string GetState(PublishedResourceEnvironment environment)
        //{
        //    if (environment.Version.HasValue)
        //    {
        //        if ( environment.Version.Value == )

        //    }

        //}
    }
}
