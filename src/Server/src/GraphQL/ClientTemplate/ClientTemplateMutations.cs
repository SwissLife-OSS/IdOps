using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;
using IdOps.Templates;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class ClientTemplateMutations
    {
        [AuthorizeClientAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<SaveClientTemplatePayload> SaveClientTemplateAsync(
            [Service] IClientTemplateService clientTemplateService,
            SaveClientTemplateRequest input,
            CancellationToken cancellationToken)
        {
            ClientTemplate clientTemplate = await clientTemplateService.SaveClientTemplate(
                input,
                cancellationToken);

            return new SaveClientTemplatePayload(clientTemplate);
        }
    }
}
