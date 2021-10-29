using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken);
        Task<Tenant> SaveAsync(SaveTenantRequest request, CancellationToken cancellationToken);
    }

    public record TenantRole(string TenantId, string Role)
    {
        public Guid? EnvironmentId { get; init; }
    }
}
