using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;
using IdOps.Templates;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class ClientTemplateQueries
    {
        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public async Task<IEnumerable<ClientTemplate>> GetClientTemplatesAsync(
            [Service] IClientTemplateService clientTemplateService,
            CancellationToken cancellationToken)
        {
            return await clientTemplateService.GetAllAsync(cancellationToken);
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: true)]
        public async Task<ClientTemplate> GetClientTemplateByIdAsync(
            [Service] IClientTemplateService clientTemplateService,
            Guid id,
            CancellationToken cancellationToken)
        {
            return await clientTemplateService.GetByIdAsync(id, cancellationToken);
        }

        [AuthorizeClientAuthoring(AccessMode.Read, includeTenantAuth: false)]
        public async Task<IReadOnlyCollection<ClientTemplateSecret>> GetSecretsAsync(
            [Service] IEnvironmentService environmentService,
            CancellationToken cancellationToken)
        {
            List<ClientTemplateSecret> result = new List<ClientTemplateSecret>();

            IEnumerable<Model.Environment>? envs =
                await environmentService.GetAllAsync(cancellationToken);

            foreach (Model.Environment? env in envs)
            {
                result.Add(new ClientTemplateSecret
                {
                    EnvironmentId = env.Id
                });
            }

            return result;
        }
    } 
}
