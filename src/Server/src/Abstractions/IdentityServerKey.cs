using System;

namespace IdOps
{
    public record IdentityServerKey(string Kid, string Alg)
    {
        public string? Thumbprint { get; init; }
        public string? SerialNumber { get; init; }
        public string? Subject { get; init; }
        public DateTime? ValidUntil { get; init; }
    }
}
