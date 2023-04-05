namespace IdOps.Abstractions
{
    public interface IIdentityRequestStore
    {
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityRequestItem> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IdentityRequestItem> SaveAsync(
            SaveIdentityRequestInput request,
            CancellationToken cancellationToken);

        Task<IEnumerable<IdentityRequestItem>> SearchAsync(
            SearchIdentityRequest searchRequest,
            CancellationToken cancellationToken);
    }

    public record SaveIdentityRequestInput(
        IdentityRequestType Type,
        string Name)
    {
        public Guid? Id { get; init; }

        public IEnumerable<string>? Tags { get; set; } = new List<string>();

        public IdentityRequestItemData Data { get; init; } = new IdentityRequestItemData();
    }


    public record SearchIdentityRequest(IdentityRequestType Type)
    {
        public string? SearchText { get; init; }

        public string? Tag { get; init; }
    }

    public class IdentityRequestItem
    {
        public Guid Id { get; set; }

        public IdentityRequestType Type { get; set; }

        public string Name { get; set; }

        public IList<string> Tags { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public IdentityRequestItemData? Data { get; set; }
    }

    public class IdentityRequestItemData
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string? Secret { get; set; }

        public string? GrantType { get; set; }

        public IEnumerable<string>? Scopes { get; set; }

        public int? Port { get; set; }

        public bool? UsePkce { get; set; }

        public bool SaveTokens { get; set; }

        public IEnumerable<TokenRequestParameter>? Parameters { get; set; }
    }

    public enum IdentityRequestType
    {
        Token,
        Authorize
    }
}
