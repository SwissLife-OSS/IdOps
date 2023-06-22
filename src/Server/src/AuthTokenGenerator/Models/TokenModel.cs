using System;
using System.Collections.Generic;
using IdOps.Abstractions;

namespace IdOps.Models
{
    public class TokenModel
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public IEnumerable<TokenClaim> Claims { get; set; }
        public int ExpiresIn { get; set; }
        public string? Subject { get; set; }
        public string? Token { get; set; }
        public TokenType? TokenType { get; set; }
    }

    public enum TokenType
    {
        Jwt,
        Reference
    }
}
