using System;
using System.Collections.Generic;
using IdOps.Abstractions;

namespace IdOps.Models
{
    public record ClaimCategoryMap(string Type, ClaimCategory Category)
    {
        public bool Hide { get; init; }
    }

    public enum ClaimCategory
    {
        Protocol,
        Payload
    }

    public class TokenModel
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public IEnumerable<TokenClaim> Claims { get; set; }
        public int ExpiresIn { get; set; }
        public string? Subject { get; set; }
        public string? Token { get; set; }
        public string? TokenType { get; set; }
    }

    public record TokenClaim(string Type, string Value)
    {
        public ClaimCategory Category { get; init; }
    }

    public class AuthenticationSessionInfo
    {
        public IDictionary<string, string?>? Properties { get; set; }

        public UserInfoResult? UserInfo { get; set; }
        public TokenModel? AccessToken { get; set; }
        public TokenModel? IdToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
