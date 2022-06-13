using System;
using System.Collections.Generic;

namespace IdOps
{
    public interface ISearchResult<out TItems>
    {
        IEnumerable<TItems> Items { get; }
    }

    public record SearchResult<TItems>(IEnumerable<TItems> Items, bool HasMore)
        : ISearchResult<TItems>;

    public record SearchPersonalAccessTokensRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public string? SearchText { get; init; }

        public IEnumerable<string>? Tenants { get; init; }

        public Guid? EnvironmentId { get; init; }
    }

    public record SearchClientsRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public string? SearchText { get; init; }

        public IEnumerable<string>? Tenants { get; init; }

        public Guid? EnvironmentId { get; init; }
    }

    public record SearchApplicationsRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public string? SearchText { get; set; }

        public IReadOnlyList<string> Tenants { get; init; }
    }

    public record SearchResourceAuditRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public Guid? ResourceId { get; init; }

        public string? UserId { get; init; }
    }

    public record SearchIdentityServerEventsRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public IEnumerable<Guid>? Applications { get; init; }

        public IEnumerable<string>? Clients { get; init; }

        public IEnumerable<string>? Environments { get; init; }

        public IEnumerable<string>? EventTypes { get; init; }
    }

    public record SearchResourcePublishLogsRequest(int PageSize, int PageNr)
    : PagedRequest(PageSize, PageNr)
    {
        public Guid? ResourceId { get; set; }

        public Guid? EnvironmentId { get; set; }
    }

    public record SearchResourceApprovalLogsRequest(int PageSize, int PageNr)
        : PagedRequest(PageSize, PageNr)
    {
        public Guid? ResourceId { get; set; }

        public Guid? EnvironmentId { get; set; }
    }

    public record PagedRequest(int PageSize, int PageNr);

    public record GetDependenciesRequest()
    {
        public Guid Id { get; init; }

        public string? Type { get; init; }
    }
}
