using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class ApiResourceMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveApiResourcePayload> SaveApiResourceAsync(
            [Service] IApiResourceService apiResourceService,
            SaveApiResourceRequest input,
            CancellationToken cancellationToken)
        {
            ApiResource apiResource = await apiResourceService.SaveAsync(
                input,
                cancellationToken);

            return new SaveApiResourcePayload(apiResource);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<AddApiSecretPayload> AddApiSecretAsync(
            [Service] IApiResourceService apiResourceService,
            AddApiSecretRequest input,
            CancellationToken cancellationToken)
        {
            (ApiResource apiRespource, string secret) = await apiResourceService.AddSecretAsync(
                input,
                cancellationToken);

            return new AddApiSecretPayload(apiRespource, secret);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<SaveApiResourcePayload> RemoveApiSecretAsync(
            [Service] IApiResourceService apiResourceService,
            RemoveApiSecretRequest input,
            CancellationToken cancellationToken)
        {
            ApiResource apiResource = await apiResourceService.RemoveSecretAsync(
                input,
                cancellationToken);

            return new SaveApiResourcePayload(apiResource);
        }
    }
}
