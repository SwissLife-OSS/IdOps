using System.Diagnostics;
using Duende.IdentityServer;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Model;
using OpenTelemetry.Trace;

namespace IdOps.IdentityServer;

internal static class IdOpsActivity
{
    public static Activity? StartDataConnector(
        DataConnectorOptions options,
        UserDataConnectorCallerContext context)
    {
        Activity? activity =
            Telemetry.ActivitySource.StartActivity($"DataConnector {options.Name}");
        activity?.SetTag("idops.identityserver.dataconnector.enabled", options.Enabled);

        Activity? parentOrCurrent = activity?.Parent ?? activity;
        parentOrCurrent?.SetTag("idops.identityserver.client_id", context.Client?.ClientId);
        parentOrCurrent?.SetTag("idops.identityserver.subject", context.Subject);

        return activity;
    }

    public static void EnrichDataConnectorResult(
        this Activity? activity,
        UserDataConnectorResult result)
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
