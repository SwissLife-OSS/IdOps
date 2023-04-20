using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Abstractions;
using IdOps.Models;

namespace IdOps.Abstractions
{
    public interface IIdentityService
    {
        Task<RequestTokenResult> RequestTokenAsync(TokenRequestData request, CancellationToken cancellationToken);
    }

    public record UserInfoResult(string? Error)
    {
        public IEnumerable<UserClaim> Claims { get; init; } = Array.Empty<UserClaim>();
    }

    public record UserClaim(string Type, string Value);

}
