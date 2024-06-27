using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdOps.Messages;
using IdOps.Model;
using IdOps.Server.Storage;
using MassTransit;

namespace IdOps.Consumers
{
    public class IdentityServerEventBatchConsumer : IConsumer<Batch<IdentityEventMessage>>
    {
        private readonly IIdentityServerEventMapper _mapper;
        private readonly IIdentityServerEventStore _eventStore;

        public IdentityServerEventBatchConsumer(
            IIdentityServerEventMapper mapper,
            IIdentityServerEventStore eventStore)
        {
            _mapper = mapper;
            _eventStore = eventStore;
        }

        public async Task Consume(ConsumeContext<Batch<IdentityEventMessage>> context)
        {
            Activity? activity = Activity.Current;
            var activityStarted = false;

            try
            {
                if (activity == null)
                {
                    activity = IdOpsActivity.StartEventBatchConsumer(context);
                    activityStarted = true;
                }
                else
                {
                    activity.EnrichStartEventBatchConsumer(context);
                }

                IdentityServerEvent?[] events = context.Message
                    .AsParallel()
                    .Select(m => _mapper.CreateEvent(m.Message))
                    .Where(m => m is not null)
                    .ToArray();

                if (events.Length > 0)
                {
                    IdOpsMeters.RecordReceiverBatchSize(events.Length);
                    await _eventStore.CreateManyAsync(events, context.CancellationToken);
                }
            }
            finally
            {
                if (activityStarted)
                {
                    activity?.Dispose();
                }
            }
        }
    }
}
