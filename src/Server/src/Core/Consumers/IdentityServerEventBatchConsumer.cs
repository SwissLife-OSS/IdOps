using System.Collections.Generic;
using System.Threading.Tasks;
using IdOps.Messages;
using IdOps.Model;
using IdOps.Store;
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
            var events = new List<IdentityServerEvent>();

            foreach (ConsumeContext<IdentityEventMessage>? message in context.Message)
            {
                IdentityServerEvent? ev = _mapper.CreateEvent(message.Message);
                if (ev is { })
                {
                    events.Add(ev);
                }
            }

            if (events.Count > 0)
            {
                await _eventStore.CreateManyAsync(events, context.CancellationToken);
            }
        }
    }
}
