using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Query)]
    public class EnvironmentQueries
    {
        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceAuthoringRead)]
        public async Task<IEnumerable<Environment>> GetEnvironmentsAsync(
            [Service] IEnvironmentService environmentService,
            CancellationToken cancellationToken)
        {
            return await environmentService.GetAllAsync(cancellationToken);
        }
    }
}
