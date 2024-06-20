using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Storage;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using static Duende.IdentityServer.IdentityServerConstants.ProfileDataCallers;

namespace IdOps.IdentityServer.DataConnector
{
    public class UserDataConnectorService : IUserDataConnectorService
    {
        private readonly ILogger<UserDataConnectorService> _logger;
        private readonly IEnumerable<IUserDataConnector> _connectors;
        private readonly IEventService _eventService;

        private static readonly Dictionary<string, ConnectorProfileType> ProfileTypeMap
            = new Dictionary<string, ConnectorProfileType>
            {
                [UserInfoEndpoint] = ConnectorProfileType.UserInfo,
                [ClaimsProviderAccessToken] = ConnectorProfileType.AccessToken,
                [ClaimsProviderIdentityToken] = ConnectorProfileType.IdentityToken,
            };

        public UserDataConnectorService(
            ILogger<UserDataConnectorService> logger,
            IEnumerable<IUserDataConnector> connectors,
            IEventService eventService)
        {
            _logger = logger;
            _connectors = connectors;
            _eventService = eventService;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(
            UserDataConnectorCallerContext context,
            IEnumerable<Claim> input,
            CancellationToken cancellationToken)
        {
            var newClaims = new List<Claim>();
            context.Subject = input.FirstOrDefaultValue(JwtClaimTypes.Subject);

            if (context?.Client?.DataConnectors != null)
            {
                foreach (DataConnectorOptions? options in context.Client.DataConnectors)
                {
                    using Activity? activity = IdOpsActivity.StartDataConnector(options, context);

                    try
                    {
                        if (!ShouldExecute(options, context.Name))
                        {
                            continue;
                        }

                        IEnumerable<Claim>? claims = await LoadDataAsync(
                            context,
                            input.Concat(newClaims),
                            options,
                            activity,
                            cancellationToken);

                        await _eventService.RaiseAsync(new UserDataConnectorSuccessEvent(
                            context.Client.ClientId,
                            context.Subject,
                            options.Name,
                            context.Name,
                            claims.Count()));

                        newClaims.AddRange(claims);
                    }
                    catch (Exception ex)
                    {
                        _logger.DataConnectorClaimsFailed(options.Name);
                        activity?.RecordException(ex);

                        await _eventService.RaiseAsync(new UserDataConnectorFailedEvent(
                            context.Client.ClientId,
                            context.Subject,
                            options.Name,
                            context.Name,
                            ex));
                    }
                }
            }

            return newClaims;
        }

        private async Task<IEnumerable<Claim>> LoadDataAsync(
            UserDataConnectorCallerContext context,
            IEnumerable<Claim> input,
            DataConnectorOptions options,
            Activity? activity,
            CancellationToken cancellationToken)
        {
            IUserDataConnector? connector = _connectors
                .Single(x => x.Name == options.Name);

            var timeOutToken = new CancellationTokenSource(300000);

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeOutToken.Token);

            UserDataConnectorResult result = await connector
                .GetClaimsAsync(context, options, input, cts.Token);

            IEnumerable<Claim> claims = result.Claims;

            activity?.EnrichDataConnectorResult(result);

            return claims;
        }

        private bool ShouldExecute(DataConnectorOptions options, string caller)
        {
            return options.Enabled &&
                   options.ProfileTypeFilter != null &&
                   options.ProfileTypeFilter.Contains(ProfileTypeMap[caller]);
        }
    }
}
