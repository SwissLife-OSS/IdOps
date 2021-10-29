using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.IdentityServer.DataConnector
{
    public interface IUserDataConnectorService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync(
            UserDataConnectorCallerContext context,
            IEnumerable<Claim> input,
            CancellationToken cancellationToken);
    }
}
