using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdOps.IdentityServer.Abstractions;
using IdOps.Messages;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdOps.IdentityServer.Azure;

public sealed class EventHubSender : BackgroundService, IEventSenderWorker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ChannelReader<IdentityEventMessage> _channelReader;

    public EventHubSender(
        IServiceProvider serviceProvider,
        ChannelReader<IdentityEventMessage> channelReader)
    {
        _serviceProvider = serviceProvider;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        IEventHubProducerProvider provider =
            scope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();

        IEventHubProducer producer = await provider.GetProducer("identity-events");

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
