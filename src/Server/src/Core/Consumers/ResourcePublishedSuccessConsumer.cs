using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Messages;
using IdOps.Model;
using IdOps.Store;
using MassTransit;

namespace IdOps.Consumers
{

    public record UiConsoleMessage(string Severity, string Type, string Message)
    {
        public string? JobId { get; init; }

        public Dictionary<string, string> Data { get; init; } = new Dictionary<string, string>();
    }

    public class ResourcePublishedErrorConsumer : IConsumer<ResourcePublishedErrorMessage>
    {
        private readonly IResourcePublishStateStore _resourcePublishStateStore;
        private readonly IResourcePublishLogStore _resourcePublishLog;
        private readonly IEnvironmentService _environmentService;

        public ResourcePublishedErrorConsumer(
            IResourcePublishStateStore resourcePublishStateStore,
            IResourcePublishLogStore resourcePublishLog,
            IEnvironmentService environmentService)
        {
            _resourcePublishStateStore = resourcePublishStateStore;
            _resourcePublishLog = resourcePublishLog;
            _environmentService = environmentService;
        }

        public async Task Consume(ConsumeContext<ResourcePublishedErrorMessage> context)
        {
            Model.Environment? environment = await _environmentService.GetByNameAsync(
                context.Message.EnvironmentName,
                context.CancellationToken);

            if (environment == null)
            {
                throw new InvalidOperationException(
                    $"Environment with name {context.Message.EnvironmentName} does not exist.");
            }

            ResourcePublishedErrorMessage message = context.Message;

            await _resourcePublishLog.CreateAsync(new ResourcePublishLog
            {
                Id = Guid.NewGuid(),
                ResourceId = message.ResourceId,
                ResourceType = message.ResourceType,
                EnvironmentId = environment.Id,
                IdentityServerGroupId = message.IdentityServerGroupId,
                PublishedAt = DateTime.UtcNow,
                RequestedBy = message.RequestedBy,
                Operation = "Error",
                ErrorMessage = message.ErrorMessage
            },
            context.CancellationToken);
        }
    }

    public class ResourcePublishedSuccessConsumer : IConsumer<ResourcePublishedSuccessMessage>
    {
        private readonly IResourcePublishStateStore _resourcePublishStateStore;
        private readonly IResourcePublishLogStore _resourcePublishLog;
        private readonly IEnvironmentService _environmentService;

        public ResourcePublishedSuccessConsumer(
            IResourcePublishStateStore resourcePublishStateStore,
            IResourcePublishLogStore resourcePublishLog,
            IEnvironmentService environmentService)
        {
            _resourcePublishStateStore = resourcePublishStateStore;
            _resourcePublishLog = resourcePublishLog;
            _environmentService = environmentService;
        }

        public async Task Consume(ConsumeContext<ResourcePublishedSuccessMessage> context)
        {
            Model.Environment? environment = await _environmentService.GetByNameAsync(
                context.Message.EnvironmentName,
                context.CancellationToken);

            if (environment == null)
            {
                throw new InvalidOperationException(
                    $"Environment with name {context.Message.EnvironmentName} does not exist.");
            }

            ResourcePublishedSuccessMessage message = context.Message;

            await _resourcePublishStateStore.SaveAsync(new ResourcePublishState
            {
                ResourceId = message.ResourceId,
                ResourceType = message.ResourceType,
                EnvironmentId = environment.Id,
                PublishedAt = DateTime.UtcNow,
                Version = message.NewVersion
            },
            context.CancellationToken);

            await _resourcePublishLog.CreateAsync(new ResourcePublishLog
            {
                Id = Guid.NewGuid(),
                ResourceId = message.ResourceId,
                ResourceType = message.ResourceType,
                EnvironmentId = environment.Id,
                IdentityServerGroupId = message.IdentityServerGroupId,
                PublishedAt = DateTime.UtcNow,
                RequestedBy = message.RequestedBy,
                Version = message.NewVersion,
                Operation = message.Operation
            },
            context.CancellationToken);
        }
    }
}
