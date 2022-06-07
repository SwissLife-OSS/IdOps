using System.Diagnostics;
using IdOps.Messages;
using MassTransit;

namespace IdOps;

internal static class IdOpsActivity
{
    private static readonly ActivitySource ActivitySource = new("IdOps");

    public static Activity? StartEventBatchConsumer(ConsumeContext<Batch<IdentityEventMessage>> context)
    {
        Activity? activity = ActivitySource.StartActivity("IdentityServer Event BatchConsumer");

        activity?.EnrichStartEventBatchConsumer(context);

        return activity;
    }

    public static void EnrichStartEventBatchConsumer(
        this Activity? activity,
        ConsumeContext<Batch<IdentityEventMessage>> context)
    {
        activity?.SetTag("messaging.masstransit.batch_count", context.Message.Length);
    }
}
