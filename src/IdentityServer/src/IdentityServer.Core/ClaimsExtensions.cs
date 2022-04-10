using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdOps.IdentityServer
{
    public static class ClaimsExtensions
    {
        public static string FirstValue(this IEnumerable<Claim> claims, string type)
        {
            return claims.First(x => x.Type == type).Value;
        }

        public static string? FirstOrDefaultValue(
            this IEnumerable<Claim> claims,
            string type)
        {
            return claims.FirstOrDefault(x => x.Type == type)?.Value;
        }
    }
}
