using System.Threading;
using System.Threading.Tasks;
using IdOps.IdentityServer.DataConnector;

namespace IdOps.IdentityServer.Store
{
    public interface IUserDataConnectorDataRepository
    {
        Task<UserDataConnectorData> GetAsync(
            string key,
            string connector,
            CancellationToken cancellationToken);

        Task SaveAsync(UserDataConnectorData data, CancellationToken cancellationToken);
    }
}
