using System.Diagnostics;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Model;
using OpenTelemetry.Trace;

namespace IdOps.IdentityServer;

internal static class IdOpsActivity
{
    private static readonly ActivitySource ActivitySource = new("IdOps.IdentityServer");

    public static Activity? StartDataConnector(DataConnectorOptions options)
    {
        Activity? activity = ActivitySource.StartActivity($"DataConnector {options.Name}");

        activity?.SetTag("idops.identityserver.dataconnector.enabled", options.Enabled);

        return activity;
    }

    public static void EnrichDataConnectorResult(this Activity? activity, UserDataConnectorResult result)
    {
        if (result.Error != null)
        {
            activity?.RecordException(result.Error);
        }

        activity?.SetTag("idops.identityserver.dataconnector.executed", result.Executed);
        activity?.SetTag("idops.identityserver.dataconnector.success", result.Success);
        activity?.SetTag("idops.identityserver.dataconnector.cachekey", result.CacheKey);
    }
}
