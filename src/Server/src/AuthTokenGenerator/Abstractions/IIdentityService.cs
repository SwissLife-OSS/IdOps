using IdentityModel.Client;
using IdOps.Models;

namespace IdOps.Abstractions
{
    public interface IIdentityService
    {
        Task<RequestTokenResult> RequestTokenAsync(
            TokenRequest request,
            CancellationToken cancellationToken);
    }
}
