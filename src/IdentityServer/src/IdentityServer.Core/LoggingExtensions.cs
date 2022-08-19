using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer.DataConnector;

public static partial class LoggingExtensions
{
    [LoggerMessage(1, LogLevel.Error, "GetClaims failed for DataConnector: {dataConnectorName}")]
    public static partial void DataConnectorClaimsFailed(this ILogger logger, string dataConnectorName);

    [LoggerMessage(2, LogLevel.Warning, "Could not save connected data: {dataConnectorName}")]
    public static partial void DataConnectorSaveDataFailed(this ILogger logger, string dataConnectorName);
}
