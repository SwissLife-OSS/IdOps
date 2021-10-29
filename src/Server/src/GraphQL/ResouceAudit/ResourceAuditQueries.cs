using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using IdOps.Model;
using IdOps.Store;

namespace IdOps.GraphQL
{
    [ExtendObjectType( RootTypes.Query)]
    public class ResourceAuditQueries
    {
        private readonly IResouceAuditStore _resouceAuditStore;

        public ResourceAuditQueries(IResouceAuditStore resouceAuditStore)
        {
            _resouceAuditStore = resouceAuditStore;
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public async Task<SearchResult<ResourceAuditEvent>> SearchResourceAudits(
            SearchResourceAuditRequest input,   
            CancellationToken cancellationToken)
        {
            return await _resouceAuditStore.SearchAsync(input, cancellationToken);
        }
    }
}
