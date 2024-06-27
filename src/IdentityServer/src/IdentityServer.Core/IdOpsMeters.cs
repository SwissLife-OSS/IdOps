using System.Diagnostics.Metrics;

namespace IdOps.IdentityServer;

public static class IdOpsMeters
{
    private static readonly Histogram<int> _eventBatchSize = Telemetry.Meter
        .CreateHistogram<int>(
            "idops.events.sender.batch_size",
            "{event}",
            "The size of the event batch being sent");

    public static void RecordSenderBatchSize(int size)
    {
        _eventBatchSize.Record(size);
    }
}
