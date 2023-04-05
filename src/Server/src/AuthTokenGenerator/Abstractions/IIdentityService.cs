using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Abstractions;

namespace IdOps.Abstractions
{
    public interface IIdentityService
    {
        Task<UserInfoResult> GetUserInfoAsync(
            string token,
            CancellationToken cancellationToken);
        Task<IEnumerable<TokenInfo>> RefreshTokenAsync(IdentityRequestItemData identityRequest, string refreshToken, CancellationToken cancellationToken);
        Task<RequestTokenResult> RequestTokenAsync(TokenRequestData request, CancellationToken cancellationToken);
    }

    public record UserInfoResult(string? Error)
    {
        public IEnumerable<UserClaim> Claims { get; init; } = Array.Empty<UserClaim>();
    }

    public record UserClaim(string Type, string Value);

}
