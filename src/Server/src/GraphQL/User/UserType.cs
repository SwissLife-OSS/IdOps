using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using IdOps.Security;

namespace IdOps.GraphQL
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field("permissions")
                .ResolveWith<Resolvers>(_ => _.GetPermissions(default!));
        }

        class Resolvers
        {
            public IEnumerable<string> GetPermissions(
                [Service] IUserContextAccessor userContextAccessor)
            {
                IUserContext context = userContextAccessor.Context ??
                    throw CouldNotAccessUserContextException.New();

                return context.Permissions;
            }
        }
    }
}
