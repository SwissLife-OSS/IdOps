using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Abstractions;
using IdOps.Encryption;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class ClientMutations
    {
        private readonly IClientService _clientService;

        public ClientMutations(IClientService clientService)
        {
            _clientService = clientService;
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveClientPayload> CreateClientAsync(CreateClientRequest input,
            CancellationToken cancellationToken)
        {
            Client client = await _clientService.CreateClientAsync(input, cancellationToken);

            return new SaveClientPayload(client);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveClientPayload> UpdateClientAsync(UpdateClientRequest input,
            CancellationToken cancellationToken)
        {
             Client client = await _clientService.UpdateClientAsync(input, cancellationToken);

            return new SaveClientPayload(client);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<AddClientSecretPayload> AddClientSecretAsync(AddClientSecretRequest input,
            CancellationToken cancellationToken)
        {
            (Client client, string secret) result =
                await _clientService.AddClientSecretAsync(input, cancellationToken);

            return new AddClientSecretPayload(result.client, result.secret);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<SaveClientPayload> RemoveClientSecretAsync(
            RemoveClientSecretRequest input, CancellationToken cancellationToken)
        {
            IdOps.Model.Client client = await _clientService.RemoveClientSecretAsync(input, cancellationToken);

            return new SaveClientPayload(client);
        }

        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: false)]
        public async Task<RequestTokenPayload> RequestTokenAsync(
            [Service] IIdentityService identityService,
            [Service] IEncryptionService encryptionService, TokenRequestInput input,
            CancellationToken cancellationToken)
        {
            Client? client = await _clientService.GetByIdAsync(input.ClientId, cancellationToken);

            Secret clientSecret =
                client.ClientSecrets.First(secret => secret.Id.Equals(input.SecretId));
            var secret =
                await encryptionService.DecryptAsync(clientSecret.EncryptedSecret,
                    cancellationToken);

            var grantTypes = client.AllowedGrantTypes.First();

            var scopes = client.AllowedScopes.Cast<string>();

            var tokenRequestData = new TokenRequestData(input.Authority, input.ClientId.ToString(),
                secret, grantTypes, scopes, input.Parameters)
            {
                RequestId = input.RequestId, SaveTokens = input.SaveTokens
            };


            RequestTokenResult tokenResult =
                await identityService.RequestTokenAsync(tokenRequestData, cancellationToken);

            return new RequestTokenPayload(tokenResult);
        }
    }
}
