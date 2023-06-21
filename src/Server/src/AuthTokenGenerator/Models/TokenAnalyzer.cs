using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdOps.Models
{
    public class TokenAnalyzer : ITokenAnalyzer
    {
         private readonly IList<ClaimCategoryMap> _categoryMap;

        public TokenAnalyzer()
        {
            _categoryMap = new List<ClaimCategoryMap>
            {
                new ClaimCategoryMap("iss", ClaimCategory.Protocol),
                new ClaimCategoryMap("nbf", ClaimCategory.Protocol) { Hide = true },
                new ClaimCategoryMap("exp", ClaimCategory.Protocol) { Hide = true },
                new ClaimCategoryMap("iat", ClaimCategory.Protocol),
                new ClaimCategoryMap("amr", ClaimCategory.Protocol),
                new ClaimCategoryMap("auth_time", ClaimCategory.Protocol),
                new ClaimCategoryMap("client_id", ClaimCategory.Protocol),
                new ClaimCategoryMap("sid", ClaimCategory.Protocol),
                new ClaimCategoryMap("jti", ClaimCategory.Protocol)
            };
        }

        public TokenModel? Analyze(string token)
        {
            if (!token.Contains("."))
            {
                return new TokenModel
                {
                    Token = token,
                    TokenType = "Reference"
                };
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt = handler.ReadToken(token) as JwtSecurityToken;

            if (jwt is { })
            {
                var model = new TokenModel
                {
                    Token = token,
                    TokenType = "Jwt",
                    ValidFrom = jwt.ValidFrom.ToLocalTime(),
                    ValidTo = jwt.ValidTo.ToLocalTime(),
                    Claims = GetClaims(jwt.Claims)
                        .OrderBy(x => x.Category)
                        .ThenBy(x => x.Type)
                };

                model.ExpiresIn = (int)(jwt.ValidTo - DateTime.UtcNow).TotalMinutes;
                model.Subject = jwt.Subject;

                return model;
            }

            return null;
        }

        private IEnumerable<TokenClaim> GetClaims(IEnumerable<Claim> claims)
        {
            foreach (Claim? claim in claims)
            {
                ClaimCategoryMap map = _categoryMap
                    .FirstOrDefault(x => x.Type == claim.Type) ??
                        new ClaimCategoryMap(claim.Type, ClaimCategory.Payload);

                if (!map.Hide)
                {
                    yield return new TokenClaim(claim.Type, claim.Value)
                    {
                        Category = map.Category
                    };
                }
            }
        }
    }
}
