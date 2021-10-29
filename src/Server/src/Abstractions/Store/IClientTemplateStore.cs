using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IClientTemplateStore
    {
        Task<IEnumerable<ClientTemplate>> GetAllAsync(
            IEnumerable<string>? tenants,
            CancellationToken cancellationToken);

        Task<ClientTemplate> GetByIdAsync(Guid id,
            CancellationToken cancellationToken);

        Task<IEnumerable<ClientTemplate>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

        Task SaveClientTemplate(ClientTemplate clientTemplate,
            CancellationToken cancellationToken);
    }
}
