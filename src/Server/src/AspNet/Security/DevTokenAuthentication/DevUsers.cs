using System.Collections.Generic;
using System.Security.Claims;

namespace IdOps.Api.Security
{
    public static class DevUsers
    {
        public static ClaimsIdentity Create(string token)
        {
            List<Claim> claims = new()
            {
                new Claim("sub", token),
                new Claim("name", token),
            };

            switch (token.ToLower())
            {
                case "contoso_dev":
                    claims.Add(new Claim("role", "IdOps.Contoso.Developer"));
                    break;
                case "contoso_admin":
                    claims.Add(new Claim("role", "IdOps.Contoso.Admin"));
                    claims.Add(new Claim("role", "IdOps.Edit"));
                    break;
                case "admin":
                    claims.Add(new Claim("role", "IdOps.Admin"));
                    break;
                case "noaccess":
                    break;
                default:
                    claims.Add(new Claim("role", "IdOps.Admin"));
                    break;
            }

            return new ClaimsIdentity(claims, nameof(DevTokenAuthorizationHandler));
        }
    }
}
