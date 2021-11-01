using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Server.Storage;

namespace IdOps
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentStore _environmentStore;

        public EnvironmentService(IEnvironmentStore environmentStore)
        {
            _environmentStore = environmentStore;
        }

        public async Task<IReadOnlyList<Model.Environment>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _environmentStore.GetAllAsync(cancellationToken);
        }

        public async Task<Model.Environment?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Model.Environment> all = await _environmentStore
                .GetAllAsync(cancellationToken);

            return all
                .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
        }

        public async Task<Model.Environment> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _environmentStore.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Model.Environment> SaveAsync(
            SaveEnvironmentRequest request,
            CancellationToken cancellationToken)
        {
            var environment = new Model.Environment
            {
                Id = request.Id.GetValueOrDefault(Guid.NewGuid()),
                Name = request.Name,
                Order = request.Order
            };

            return await _environmentStore.SaveAsync(environment, cancellationToken);
        }
    }
}
