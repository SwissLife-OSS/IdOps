using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdOps.Abstractions
{
    public interface IAuthTokenStore
    {
        Task StoreAsync(TokenStoreModel model, CancellationToken cancellationToken);

        Task<TokenStoreModel> GetAsync(string name, CancellationToken cancellationToken);
        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }

    public record TokenStoreModel(string Name, DateTime CreatedAt)
    {
        public IList<TokenInfo> Tokens { get; init; } = new List<TokenInfo>();

        public Guid? RequestId { get; set; }
    }

    public record TokenStoreHeader(string Id, string Name, DateTime CreatedAt)
    {
        public bool HasAccessToken { get; set; }

        public bool HasIdToken { get; set; }
        public bool HasRefreshToken { get; set; }

        public Guid? RequestId { get; set; }
        public int AccessTokenExpiresIn { get; set; }
    }

    public record TokenInfo(TokenType Type, string Token)
    {
        public DateTimeOffset? ExpiresAt { get; init; }
    }

    public enum TokenType
    {
        Access,
        Id,
        Refresh
    }
}
