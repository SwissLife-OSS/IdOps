using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;

namespace IdOps.GraphQL.Publish
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class PublishMutations
    {
        private readonly IResourcePublisher _resourcePublisher;

        public PublishMutations(IResourcePublisher resourcePublisher)
        {
            _resourcePublisher = resourcePublisher;
        }

        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceAuthoringPublish)]
        public async Task<PublishResourcePayload> PublishResourceAsync(
            PublishResourceRequest input,
            CancellationToken cancellationToken)
        {
            PublishResourcesResult result = await
                _resourcePublisher.PublishResourcesAsync(input, cancellationToken);

            return new PublishResourcePayload(result.JobId, result.Resources);
        }
    }

    public class PublishResourcePayload : Payload
    {
        public PublishResourcePayload(Guid jobId, IEnumerable<Guid> resources)
        {
            JobId = jobId;
            Resources = resources;
        }

        public Guid JobId { get; }
        public IEnumerable<Guid> Resources { get; }
    }
}
