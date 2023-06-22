using IdOps.Models;

namespace IdOps.Abstractions
{
    public interface IIdentityService
    {
        Task<RequestTokenResult> RequestTokenAsync(
            TokenRequestData request, 
            CancellationToken cancellationToken);
    }
}
