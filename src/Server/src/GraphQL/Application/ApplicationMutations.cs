using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(Name = RootTypes.Mutation)]
    public class ApplicationMutations
    {
        private readonly IApplicationService _applicationService;

        public ApplicationMutations(
            IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<CreateApplicationPayload> CreateApplicationAsync(
            CreateApplicationRequest input,
            CancellationToken cancellationToken)
        {
            CreateAplicationResult? result = await _applicationService.CreateAsync(
                input,
                cancellationToken);

            return new CreateApplicationPayload(
                result.Application,
                result.Clients);
        }

        public async Task<UpdateApplicationPayload> UpdateApplicationAsync(
            UpdateApplicationRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.UpdateAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        public async Task<UpdateApplicationPayload> RemoveClientFromApplicationAsync(
            RemoveClientRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.RemoveClientAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        public async Task<UpdateApplicationPayload> AddClientToApplicationAsync(
            AddClientRequest input,
            CancellationToken cancellationToken)
        {
            Application application = await _applicationService.AddClientAsync(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }

        public async Task<UpdateApplicationPayload> AddEnvironmentToApplicationAsync(
            AddEnvironmentToApplicationRequest input, CancellationToken cancellationToken)
        {
            Application application = await _applicationService.AddEnvironmentToApplicationAsnyc(
                input,
                cancellationToken);

            return new UpdateApplicationPayload(application);
        }
    }
}
