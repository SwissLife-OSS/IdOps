using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Authorization;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Mutation)]
    public class ApplicationMutations
    {
        private readonly IApplicationService _applicationService;

        public ApplicationMutations(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<CreateApplicationPayload> CreateApplicationAsync(
            CreateApplicationRequest input,
            CancellationToken cancellationToken)
        {
            ApplicationWithClients? result = await _applicationService.CreateAsync(
                input,
                cancellationToken);

            return new CreateApplicationPayload(
                result.Application,
                result.Clients);
        }

        [AuthorizeClientAuthoring(AccessMode.Write)]
        public async Task<UpdateApplicationPayload> UpdateApplicationAsync(
            UpdateApplicationRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.UpdateAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        [AuthorizeClientAuthoring(AccessMode.Write)]
        public async Task<UpdateApplicationPayload> RemoveClientFromApplicationAsync(
            RemoveClientRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.RemoveClientAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        [AuthorizeClientAuthoring(AccessMode.Write)]
        public async Task<UpdateApplicationPayload> AddClientToApplicationAsync(
            AddClientRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.AddClientAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        [AuthorizeClientAuthoring(AccessMode.Write)]
        public async Task<AddEnvironmentToApplicationPayload> AddEnvironmentToApplicationAsync(
            AddEnvironmentToApplicationRequest input,
            CancellationToken cancellationToken)
        {
            ApplicationWithClients? result =
                await _applicationService.AddEnvironmentToApplicationAsync(
                    input,
                    cancellationToken);

            return new AddEnvironmentToApplicationPayload(result.Application, result.Clients);
        }
    }
}
