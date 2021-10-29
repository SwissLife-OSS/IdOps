using Microsoft.AspNetCore.Authorization;

namespace IdOps.Authorization
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public HasPermissionRequirement(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }
    }
}
