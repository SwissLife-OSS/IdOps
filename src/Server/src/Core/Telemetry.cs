using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace IdOps;

internal static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("IdOps");
    
    public static readonly Meter Meter = new("IdOps");
}
