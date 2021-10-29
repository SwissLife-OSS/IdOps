using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

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

        public static string ToJson(this IEnumerable<Claim> claims)
        {
            JObject data = new JObject();
            foreach (IGrouping<string, Claim>? group in claims.GroupBy(x => x.Type))
            {
                if (group.Count() == 1)
                {
                    data.Add(group.Key, group.First().Value);
                }
                else
                {
                    data.Add(group.Key, new JArray(group.Select(x => x.Value)));
                }
            }

            return data.ToString();
        }
    }
}
