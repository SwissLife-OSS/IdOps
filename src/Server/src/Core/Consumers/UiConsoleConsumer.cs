using System.Threading.Tasks;
using IdOps.Messages;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace IdOps.Consumers
{
    public class UiConsoleConsumer : IConsumer<ResourcePublishedSuccessMessage>
    {
        private readonly IHubContext<OpsHub> _hubContext;
        private readonly IEnvironmentService _environmentService;

        public UiConsoleConsumer(
            IHubContext<OpsHub> hubContext,
            IEnvironmentService environmentService)
        {
            _hubContext = hubContext;
            _environmentService = environmentService;
        }

        public async Task Consume(ConsumeContext<ResourcePublishedSuccessMessage> context)
        {
            ResourcePublishedSuccessMessage msg = context.Message;

            Model.Environment? environment = await _environmentService.GetByNameAsync(
                context.Message.EnvironmentName,
                context.CancellationToken);

            var message = new UiConsoleMessage(
                "SUCCESS",
                "PUBLISHED",
                $"Resource {msg.Operation} {msg.ResourceType}/{msg.ResourceId} " +
                $"published to {msg.EnvironmentName}")
            {
                JobId = msg.JobId.ToString("N"),
                Data = new()
                {
                    ["resourceId"] = context.Message.ResourceId.ToString("N"),
                    ["environmentId"] = environment.Id.ToString("N"),
                }
            };

            await _hubContext.Clients.All.SendAsync(
                "published",
                message,
                context.CancellationToken);
        }
    }
}
