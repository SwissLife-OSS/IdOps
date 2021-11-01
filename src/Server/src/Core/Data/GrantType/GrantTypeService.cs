using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;
using IdOps.Security;
using IdOps.Server.Storage;

namespace IdOps
{
    public class GrantTypeService : UserTenantService, IGrantTypeService
    {
        private readonly IGrantTypeStore _grantTypeStore;

        public GrantTypeService(
            IGrantTypeStore grantTypeStore,
            IUserContextAccessor userContextAccessor)
                : base(userContextAccessor)
        {
            _grantTypeStore = grantTypeStore;
        }

        public async Task<IEnumerable<GrantType>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            return await _grantTypeStore.GetAllAsync(cancellationToken);
        }

        public async Task<GrantType> SaveAsync(
            SaveGrantTypeRequest request,
            CancellationToken cancellationToken)
        {
            var grantType = new GrantType
            {
                Id = request.Id,
                Tenants = request.Tenants.ToList(),
                Name = request.Name,
                IsCustom = request.IsCustom,
            };

            await _grantTypeStore.SaveAsync(grantType, cancellationToken);

            return grantType;
        }
    }
}
