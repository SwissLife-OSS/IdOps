using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Query)]
    public class ApplicationQueries
    {
        private readonly IApplicationService _applicationService;

        public ApplicationQueries(
            IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<Application> GetApplicationAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _applicationService.GetByIdAsync(id, cancellationToken);
        }

        public async Task<SearchResult<Application>> SearchApplicationsAsync(
            SearchApplicationsRequest input,
            CancellationToken cancellationToken)
        {
            return await _applicationService.SearchAsync(
                input,
                cancellationToken);
        }

        public async Task<IEnumerable<Client>> SearchUnMappedClientsAsync(
            string tenant,
            CancellationToken cancellationToken)
        {
            return await _applicationService.SearchUnMappedClientsAsync(
                tenant,
                cancellationToken);
        }
    }
}
