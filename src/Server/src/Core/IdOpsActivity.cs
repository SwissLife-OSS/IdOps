using System.Diagnostics;
using IdOps.Messages;
using MassTransit;

namespace IdOps;

internal static class IdOpsActivity
{
    public static Activity? StartEventBatchConsumer(
        ConsumeContext<Batch<IdentityEventMessage>> context)
    {
        Activity? activity =
            Telemetry.ActivitySource.StartActivity("IdentityServer Event BatchConsumer");

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