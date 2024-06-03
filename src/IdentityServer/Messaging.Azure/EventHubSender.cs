using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdOps.IdentityServer.Abstractions;
using IdOps.Messages;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace IdOps.IdentityServer.Azure;

public sealed class EventHubSender : BackgroundService, IEventSenderWorker
{
    private readonly ChannelReader<IdentityEventMessage> _channelReader;
    private readonly IEventHubProducerProvider _producerProvider;

    public EventHubSender(
        IEventHubProducerProvider provider,
        ChannelReader<IdentityEventMessage> channelReader)
    {
        _producerProvider = provider;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        IEventHubProducer producer = await _producerProvider.GetProducer("identity-events");

        // we reuse the buffer to avoid allocations
        var buffer = new IdentityEventMessage[50];
        try
        {
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                // we read as many messages as we can
                for (var i = 0; i < buffer.Length; i++)
                {
                    if (!_channelReader.TryRead(out IdentityEventMessage? entity))
                    {
                        break;
                    }

                    buffer[i] = entity;
                }

                // create a batch of messages to send
                var batch = new IdentityEventMessage[buffer.Length];
                Array.Copy(buffer, batch, buffer.Length);
                buffer.AsSpan().Clear();

                IdOpsMeters.RecordSenderBatchSize(batch.Length);

                await producer.Produce<IdentityEventMessage>(batch, stoppingToken);
            }
        }
        catch
        {
            // ignored
        }
    }
}
