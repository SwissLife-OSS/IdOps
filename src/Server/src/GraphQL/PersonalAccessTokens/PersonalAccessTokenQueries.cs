using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class PersonalAccessTokenQueries
    {
        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<PersonalAccessToken> GetPersonalAccessTokenAsync(
            [Service] IPersonalAccessTokenService patService,
            Guid id,
            CancellationToken cancellationToken) =>
            await patService.GetByIdAsync(id, cancellationToken);

        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<IReadOnlyList<PersonalAccessToken>> GetPersonalAccessTokensAsync(
            [Service] IPersonalAccessTokenService patService,
            GetPersonalAccessTokensInput input,
            CancellationToken cancellationToken) =>
            await patService.GetByTenantsAsync(null, input.Tenants, cancellationToken);

        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<SearchResult<PersonalAccessToken>> SearchPersonalAccessTokensAsync(
            [Service] IPersonalAccessTokenService patService,
            SearchPersonalAccessTokensRequest input,
            CancellationToken cancellationToken) =>
            await patService.SearchAsync(input, cancellationToken);
    }
}
