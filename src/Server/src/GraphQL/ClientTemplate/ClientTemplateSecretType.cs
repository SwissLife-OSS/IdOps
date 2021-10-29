using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class ClientTemplateSecretType : ObjectType<ClientTemplateSecret>
    {
        protected override void Configure(IObjectTypeDescriptor<ClientTemplateSecret> descriptor)
        {
            descriptor.Field("environment")
                .ResolveWith<Resolvers>(_ => _.GetEnvironment(default, default, default));
        }
        class Resolvers
        {
            public async Task<string> GetEnvironment(
                [Parent] ClientTemplateSecret clientTemplateSecret,
                [Service] IEnvironmentService service,
                CancellationToken cancellationToken)
            {
                Model.Environment? env = await service.GetByIdAsync(
                    clientTemplateSecret.EnvironmentId, cancellationToken);
                return env.Name;
            }
        }
    }
}
