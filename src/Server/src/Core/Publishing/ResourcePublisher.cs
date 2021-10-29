using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IdSM = IdOps.IdentityServer.Model;
using IdOps.Messages;
using IdOps.Model;
using IdOps.Security;
using MassTransit;
using Newtonsoft.Json;


namespace IdOps
{
    public class ResourcePublisher : IResourcePublisher
    {
        private readonly IPublishingService _publishingService;
        private readonly IResourceServiceResolver _resourceServiceResolver;
        private readonly IResourceMessageFactoryResolver _messageFactoryResolver;
        private readonly IPublishedResourceDependencyResolver _dependencyResolver;
        private readonly IResourceAuthoring _resourceAuthoring;
        private readonly IUserContextAccessor _userContextAccessor;
        private readonly IPublishingContextFactory _publishingContextFactory;
        private readonly IBus _bus;

        public ResourcePublisher(
            IPublishingService publishingService,
            IResourceServiceResolver resourceServiceResolver,
            IResourceMessageFactoryResolver messageFactoryResolver,
            IPublishedResourceDependencyResolver dependencyResolver,
            IResourceAuthoring resourceAuthoring,
            IUserContextAccessor userContextAccessor,
            IPublishingContextFactory publishingContextFactory,
            IBus bus)
        {
            _publishingService = publishingService;
            _resourceServiceResolver = resourceServiceResolver;
            _messageFactoryResolver = messageFactoryResolver;
            _dependencyResolver = dependencyResolver;
            _resourceAuthoring = resourceAuthoring;
            _userContextAccessor = userContextAccessor;
            _publishingContextFactory = publishingContextFactory;
            _bus = bus;
        }

        public async Task<PublishResourcesResult> PublishResourcesAsync(
            PublishResourceRequest request,
            CancellationToken cancellationToken)
        {
            IEnumerable<PublishedResource> requestedResources =
                await BuildPublishingPlanAsync(request, cancellationToken);

            Model.Environment destEnv =
                await _resourceAuthoring.Environments
                    .GetByIdAsync(request.DestinationEnvionmentId, cancellationToken);

            Guid jobId = Guid.NewGuid();
            IPublishingContext context = await _publishingContextFactory
                .CreateAsync(request.DestinationEnvionmentId, cancellationToken);

            foreach (PublishedResource publishedResource in requestedResources)
            {
                PublishedResourceEnvironment? envState = publishedResource.Environments
                    .FirstOrDefault(x => x.EnvironmentId == request.DestinationEnvionmentId);

                if (envState?.State is ResourceStates.NotApproved)
                {
                    continue;
                }

                if (!TryResolverServiceAndFactory(
                    publishedResource.Type,
                    out IResourceService? resourceService,
                    out IResourceMessageFactory? messageFactory))
                {
                    continue;
                }

                // Task: Resolve Clients from Application OnSave and not OnPublish (UserClaimRules)
                if (envState?.State is ResourceStates.Latest && !resourceService.ForcePublish)
                {
                    continue;
                }

                IResource? resource = await resourceService
                    .GetResourceByIdAsync(publishedResource.Id, cancellationToken);

                if (resource is null)
                {
                    continue;
                }

                IdSM.IdOpsResource? payload = await messageFactory
                    .BuildPublishMessageAsync(context, resource, cancellationToken);

                if (payload is null)
                {
                    continue;
                }

                payload.Source = PublisherHelper.CreateSource(resource);

                IdentityServerGroup? serverGroup = await messageFactory
                    .ResolveServerGroupAsync(resource, cancellationToken);

                if (serverGroup is null)
                {
                    continue;
                }

                PublishResourceMessage message =
                    CreatePublishMessage(publishedResource, serverGroup.Id, payload, jobId);

                Uri uri = new($"queue:id-{serverGroup.Name.ToLower()}-{destEnv.Name.ToLower()}");

                ISendEndpoint sender = await _bus.GetSendEndpoint(uri);

                await sender.Send(message, cancellationToken);
            }

            return new PublishResourcesResult(jobId, requestedResources.Select(x => x.Id));
        }

        private bool TryResolverServiceAndFactory(
            string type,
            [NotNullWhen(true)] out IResourceService? service,
            [NotNullWhen(true)] out IResourceMessageFactory? factory)
        {
            factory = null;
            return _resourceServiceResolver.TryResolveService(type, out service) &&
                _messageFactoryResolver.TryResolverFactory(type, out factory);
        }

        private async Task<IEnumerable<PublishedResource>> BuildPublishingPlanAsync(
            PublishResourceRequest request,
            CancellationToken cancellationToken)
        {
            PublishedResourcesRequest publishedRequest = new() { ResourceId = request.Resources };
            IReadOnlyList<PublishedResource> requestedResources = await _publishingService
                .GetPublishedResourcesAsync(publishedRequest, cancellationToken)
                .ToListAsync(cancellationToken);

            IReadOnlyList<Guid> dependencies =
                await BuildDependenciesAsync(requestedResources, cancellationToken);

            IReadOnlyList<PublishedResource> requestedDependencies =
                Array.Empty<PublishedResource>();
            if (dependencies.Count > 0)
            {
                PublishedResourcesRequest dependencyRequest = new() { ResourceId = dependencies };
                requestedDependencies = await _publishingService
                    .GetPublishedResourcesAsync(dependencyRequest, cancellationToken)
                    .ToListAsync(cancellationToken);
            }

            return requestedResources.Concat(requestedDependencies);
        }

        private async Task<IReadOnlyList<Guid>> BuildDependenciesAsync(
            IEnumerable<PublishedResource> requested,
            CancellationToken cancellationToken)
        {
            HashSet<Guid> collectedIds = requested.Select(x => x.Id).ToHashSet();
            List<Guid>? dependencies = null;

            foreach (PublishedResource resource in requested)
            {
                IReadOnlyList<IResource> resourceDependencies = await _dependencyResolver
                    .ResolveDependenciesAsync(resource, cancellationToken);

                foreach (var dependency in resourceDependencies)
                {
                    if (collectedIds.Add(dependency.Id))
                    {
                        dependencies ??= new();
                        dependencies.Add(dependency.Id);
                    }
                }
            }

            if (dependencies is not null)
            {
                return dependencies;
            }

            return Array.Empty<Guid>();
        }

        private PublishResourceMessage CreatePublishMessage<T>(
            PublishedResource published,
            Guid identityServerGroupId,
            T resource,
            Guid jobId)
        {
            IUserContext user = _userContextAccessor.Context ??
                throw CouldNotAccessUserContextException.New();

            return new PublishResourceMessage
            {
                ResourceId = published.Id,
                ResourceType = published.Type,
                Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resource)),
                RequestedBy = user.UserId,
                IdentityServerGroupId = identityServerGroupId,
                JobId = jobId,
            };
        }
    }
}
