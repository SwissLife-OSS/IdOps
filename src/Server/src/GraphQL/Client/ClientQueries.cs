using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using IdOps.Data.Errors;

using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class ClientQueries
    {
        private readonly IClientService _clientService;

        public ClientQueries(IClientService clientService)
        {
            _clientService = clientService;
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public IEnumerable<string> GetClientIdGenerators()
        {
            return _clientService.GetClientIdGenerators();
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public IEnumerable<string> GetSharedSecretGenerators()
        {
            return _clientService.GetSharedSecretGenerators();
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<Client> GetClientAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _clientService.GetByIdAsync(id, cancellationToken);
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<SearchResult<Client>> SearchClientsAsync(SearchClientsRequest input,
            CancellationToken cancellationToken)
        {
            return await _clientService.SearchClientsAsync(input, cancellationToken);
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<GetClientSecretPayload> GetClientSecretAsync(GetClientSecretRequest input,
            CancellationToken cancellationToken)
        {
            try
            {
                (Client client, string secret) =
                    await _clientService.GetClientSecretAsync(input, cancellationToken);
                return new GetClientSecretPayload(client, secret);
            }
            catch (ErrorException e)
            {
                Client client =
                    await _clientService.GetByIdAsync(input.ClientId, cancellationToken);

                if (e.Error is NoEncryptedSecretError error)
                {
                    return new GetClientSecretPayload(client, "", error);
                }

                throw new InvalidOperationException();
            }
        }
    }
}
