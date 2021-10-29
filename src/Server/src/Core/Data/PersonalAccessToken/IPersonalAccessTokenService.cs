using System;
using System.Threading;
using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface IPersonalAccessTokenService : IResourceService<PersonalAccessToken>
    {
        /// <summary>
        /// Creates a new personal access token.
        /// </summary>
        /// <param name="request">The <see cref="PersonalAccessToken"/> to insert</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>
        /// A payload that contains the personal access token, the secret or errors
        /// </returns>
        Task<CreatePersonalAccessTokenResult> CreateAsync(
            CreatePersonalAccessTokenRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Updates a personal access token.
        /// </summary>
        /// <param name="request">
        /// The <see cref="UpdatePersonalAccessTokenRequest"/> to update
        /// </param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>
        /// A payload that contains the personal access token, the secret or errors
        /// </returns>
        Task<PersonalAccessToken> UpdateAsync(
            UpdatePersonalAccessTokenRequest request,
            CancellationToken cancellationToken);

        Task<SearchResult<PersonalAccessToken>> SearchAsync(
            SearchPersonalAccessTokensRequest request,
            CancellationToken cancellationToken);

        Task<PersonalAccessToken> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<AddSecretPersonalAccessTokenResult> AddSecretToTokenAsync(
            DateTime expiresAt,
            Guid id,
            CancellationToken cancellationToken);

        Task<RemoveSecretPersonalAccessTokenResult> RemoveSecretOfTokenAsync(
            Guid id,
            Guid tokenId,
            CancellationToken cancellationToken);
    }
}
