using System;
using HotChocolate.Types;
using IdOps.Security;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Query)]
    public class UserQueries
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public UserQueries(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public User GetMe()
        {
            IUserContext context = _userContextAccessor.Context ??
                throw CouldNotAccessUserContextException.New();

            return context.User;
        }
    }
}
