using System;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;
using IdOps.Messages;
using MassTransit;

namespace IdOps.IdentityServer.ResourceUpdate
{
    public class UpdateResourceConsumer : IConsumer<PublishResourceMessage>
    {
        private readonly IResourceUpdateHandler _updateHandler;
        private readonly IdOpsOptions _options;

        public UpdateResourceConsumer(
            IResourceUpdateHandler updateHandler,
            IdOpsOptions options)
        {
            _updateHandler = updateHandler;
            _options = options;
        }

        public async Task Consume(ConsumeContext<PublishResourceMessage> context)
        {
            try
            {
                UpdateResourceResult? result = await _updateHandler.HandleUpdateAsync(
                    context.Message,
                    context.CancellationToken);

                var updatedMessage = new ResourcePublishedSuccessMessage
                {
                    JobId = context.Message.JobId,
                    ResourceType = context.Message.ResourceType,
                    ResourceId = context.Message.ResourceId,
                    IdentityServerGroupId = context.Message.IdentityServerGroupId,
                    OldVersion = result.OldVersion,
                    NewVersion = result.NewVersion,
                    Operation = result.Operation.ToString(),
                    EnvironmentName = _options.EnvironmentName,
                    RequestedBy = context.Message.RequestedBy
                };

                await context.Publish(updatedMessage, context.CancellationToken);
            }
            catch (Exception ex)
            {
                var errorMessage = new ResourcePublishedErrorMessage
                {
                    JobId = context.Message.JobId,
                    ResourceType = context.Message.ResourceType,
                    ResourceId = context.Message.ResourceId,
                    IdentityServerGroupId = context.Message.IdentityServerGroupId,
                    EnvironmentName = _options.EnvironmentName,
                    RequestedBy = context.Message.RequestedBy,
                    ErrorMessage = ex.Message
                };

                await context.Publish(errorMessage, context.CancellationToken);
            }
        }
    }
}
