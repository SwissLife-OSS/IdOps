using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace IdOps.Api.Security
{
    public static class ClaimActionCollectionExtensions
    {
        public static void MapUserRoles(this ClaimActionCollection claimActions)
        {
            claimActions.Add(new JsonKeyArrayClaimAction("role", "role"));
        }
    }
}
