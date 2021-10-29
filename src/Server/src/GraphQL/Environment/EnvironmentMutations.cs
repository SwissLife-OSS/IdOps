using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Authorization;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Mutation)]
    public class EnvironmentMutations
    {
        [Authorize(
            Apply = ApplyPolicy.BeforeResolver,
            Policy = AuthorizationPolicies.Names.ResourceAuthoringEdit)]
        public async Task<SaveEnvironmentPayload> SaveEnvironmentAsync(
            SaveEnvironmentRequest input,
            [Service] IEnvironmentService environmentService,
            CancellationToken cancellationToken)
        {
            Environment environment = await environmentService.SaveAsync(input, cancellationToken);

            return new SaveEnvironmentPayload(environment);
        }
    }
}
