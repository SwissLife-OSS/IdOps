using Microsoft.AspNetCore.Authorization;

namespace IdOps.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequirePermission(
            this AuthorizationPolicyBuilder builder,
            string permission)
        {
            builder.AddRequirements(new HasPermissionRequirement(permission));

            return builder;
        }
    }
}
