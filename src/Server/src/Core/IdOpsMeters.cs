using System.Diagnostics.Metrics;

namespace IdOps;

internal static class IdOpsMeters
{
    private static readonly Histogram<int> _eventBatchSize = Telemetry.Meter
        .CreateHistogram<int>(
            "idops.events.receiver.batch_size",
            "{event}",
            "The size of the event batch being sent");

    public static void RecordReceiverBatchSize(int size)
    {
        _eventBatchSize.Record(size);
    }
}
