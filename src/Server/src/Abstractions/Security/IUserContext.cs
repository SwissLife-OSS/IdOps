using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.Security;

public interface IUserContext
{
    public bool IsAuthenticated { get; }

    string UserId { get; }

    IEnumerable<string> Roles { get; }

    User User { get; }

    IEnumerable<string> Permissions { get; }

    bool HasRole(string role);

    bool HasPermission(string permission);

    Task<IReadOnlyList<string>> GetTenantsAsync(CancellationToken ct);
}
