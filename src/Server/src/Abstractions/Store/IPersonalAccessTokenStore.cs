using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps.Store
{
    public interface IPersonalAccessTokenStore
        : ITenantResourceStore<PersonalAccessToken>
    {
        Task<PersonalAccessToken> CreateAsync(
            PersonalAccessToken token,
            CancellationToken cancellationToken);

        Task<SearchResult<PersonalAccessToken>> SearchAsync(
            SearchPersonalAccessTokensRequest request,
            CancellationToken cancellationToken);
    }
}
