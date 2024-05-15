using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdOps.IdentityServer.Abstractions;
using IdOps.Messages;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace IdOps.IdentityServer.Events;

public sealed class BusEventSender : BackgroundService, IEventSenderWorker
{
    private readonly IBus _bus;
    private readonly ChannelReader<IdentityEventMessage> _channelReader;

    public BusEventSender(
        IBus bus,
        ChannelReader<IdentityEventMessage> channelReader)
    {
        _bus = bus;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        try
        {
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                IdentityEventMessage entity = await _channelReader.ReadAsync(stoppingToken);

                await _bus.Publish(entity, stoppingToken);
            }
        }
        catch
        {
            // ignored
        }
    }
}
