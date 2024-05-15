using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace IdOps.IdentityServer;

internal static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("IdOps.IdentityServer");
    public static readonly Meter Meter = new("IdOps.IdentityServer");
}
