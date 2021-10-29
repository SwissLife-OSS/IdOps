using System.Text.Json;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class IdentityServerEventType: ObjectType<IdentityServerEvent>
    {
        protected override void Configure(
            IObjectTypeDescriptor<IdentityServerEvent> descriptor)
        {
            descriptor.Field("rawData")
                .ResolveWith<Resolvers>(_ => _.GetData(default!));
        }

        private class Resolvers
        {
            public string GetData(
                [Parent] IdentityServerEvent ev)
            {

                return JsonSerializer.Serialize(ev.Data);
            }
        }
    }
}
