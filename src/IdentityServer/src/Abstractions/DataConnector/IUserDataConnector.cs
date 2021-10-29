using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.DataConnector
{
    public interface IUserDataConnector
    {
        Task<UserDataConnectorResult> GetClaimsAsync(
            UserDataConnectorCallerContext context,
            DataConnectorOptions options,
            IEnumerable<Claim> input,
            CancellationToken cancellationToken);

        string Name { get; }
    }
}
