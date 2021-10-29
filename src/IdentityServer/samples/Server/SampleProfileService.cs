using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.Samples
{
    public class SampleProfileService : IProfileService
    {
        private readonly IUserDataConnectorService _userDataConnectorService;

        public SampleProfileService(
            IUserDataConnectorService userDataConnectorService)
        {
            _userDataConnectorService = userDataConnectorService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            IEnumerable<Claim> connectorClaims = await GetConnectorClaimsAsync(context);

            context.IssuedClaims.AddRange(context.Subject.Claims.Where(x =>
                context.RequestedClaimTypes.Contains(x.Type)));

            context.IssuedClaims.AddRange(connectorClaims); //Should filter by requestedTypes?
        }

        private async Task<IEnumerable<Claim>> GetConnectorClaimsAsync(
            ProfileDataRequestContext context)
        {
            var callerContext = new UserDataConnectorCallerContext
            {
                Client = context.Client as IdOpsClient,
                Name = context.Caller,
                RequestedClaimTypes = context.RequestedClaimTypes
            };

            IEnumerable<Claim>? connectorClaims = await _userDataConnectorService.GetClaimsAsync(
                callerContext,
                context.Subject.Claims,
                default);

            return connectorClaims;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.CompletedTask;
        }
    }
}
