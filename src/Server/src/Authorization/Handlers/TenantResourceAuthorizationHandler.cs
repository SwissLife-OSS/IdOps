using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using IdOps.Security;
using Microsoft.AspNetCore.Authorization;

namespace IdOps.Authorization
{
    public class TenantResourceAuthorizationHandler
        : AuthorizationHandler<HasTenantRequirement>
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public TenantResourceAuthorizationHandler(IUserContextAccessor userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasTenantRequirement requirement)
        {
            if (_userContextAccessor.Context is null)
            {
                context.Fail();
                return;
            }

            IUserContext user = _userContextAccessor.Context;
            IReadOnlyList<string> userTenants = await user.GetTenantsAsync(CancellationToken.None);
            IEnumerable<string> tenants = GraphQLContextTenantResolver.GetTenants(context.Resource);

            foreach (var tenant in tenants.Distinct())
            {
                if (!userTenants.Contains(tenant))
                {
                    context.Fail();
                }
            }

            context.Succeed(requirement);
        }
    }

    public static class GraphQLContextTenantResolver
    {
        public static IEnumerable<string> GetTenants(object? resource)
        {
            switch (resource)
            {
                case IMiddlewareContext gql:

                    IEnumerable<string>? fromResult = GetFromResult(gql.Result);

                    if (fromResult is { })
                    {
                        return fromResult;
                    }

                    ITenantInput? input = gql.ArgumentValue<ITenantInput>("input");

                    if (input is { })
                    {
                        return new[]
                        {
                            input.Tenant
                        };
                    }

                    break;
            }

            return Array.Empty<string>();
        }

        private static IEnumerable<string>? GetFromResult(object? result)
        {
            if (result is { })
            {
                switch (result)
                {
                    case IHasTenant c:
                        return new[] { c.Tenant };
                    case IEnumerable<IHasTenant > list:
                        return list.Select(x => x.Tenant).Distinct();
                    case ISearchResult<IHasTenant > sr:
                        return GetFromResult(sr.Items);
                }
            }

            return null;
        }
    }
}
