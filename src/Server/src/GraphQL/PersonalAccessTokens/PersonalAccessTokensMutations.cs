using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using IdOps.Model;

namespace IdOps.GraphQL
{
    [ExtendObjectType(RootTypes.Mutation)]
    public class PersonalAccessTokensMutations
    {
        /// <summary>
        /// Creates a new personal access token. The payload object contains the secret.
        /// This secret will only be available in the payload and cannot be requested.
        /// </summary>
        /// <param name="services">The <see cref="IPersonalAccessTokenService"/></param>
        /// <param name="input">The input for this field</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>
        /// A payload that contains the personal access token, the secret or errors
        /// </returns>
        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Write, includeTenantAuth: true)]
        public async Task<CreatePersonalAccessTokenPayload> CreatePersonalAccessTokenAsync(
            [Service] IPersonalAccessTokenService services,
            CreatePersonalAccessTokenRequest input,
            CancellationToken cancellationToken)
        {
            try
            {
                CreatePersonalAccessTokenResult result =
                    await services.CreateAsync(input, cancellationToken);

                return new CreatePersonalAccessTokenPayload(result.Token);
            }
            catch (ErrorException exception)
            {
                if (exception.Error is ICreatePersonalAccessTokenError error)
                {
                    return new CreatePersonalAccessTokenPayload(null, null, error);
                }

                throw new InvalidOperationException("Error was not found");
            }
        }

        /// <summary>
        /// Updates a personal access token.
        /// </summary>
        /// <param name="services">The <see cref="IPersonalAccessTokenService"/></param>
        /// <param name="input">The input for this field</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>
        /// A payload that contains the personal access token, the secret or errors
        /// </returns>
        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Write)]
        public async Task<SavePersonalAccessTokenPayload> UpdatePersonalAccessTokenAsync(
            [Service] IPersonalAccessTokenService services,
            UpdatePersonalAccessTokenRequest input,
            CancellationToken cancellationToken)
        {
            PersonalAccessToken token =
                await services.UpdateAsync(input, cancellationToken);

            return new SavePersonalAccessTokenPayload(token);
        }

        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Write)]
        public async Task<AddSecretPersonalAccessTokenPayload> AddSecretToPersonalAccessTokenAsync(
            [Service] IPersonalAccessTokenService services,
            AddSecretPersonalAccessTokenRequest input,
            CancellationToken cancellationToken)
        {
            AddSecretPersonalAccessTokenResult result =
                await services
                    .AddSecretToTokenAsync(input.ExpiresAt, input.TokenId, cancellationToken);

            return AddSecretPersonalAccessTokenPayload.From(result);
        }

        [AuthorizePersonalAccessTokenAuthoring(AccessMode.Write)]
        public async Task<RemoveSecretPersonalAccessTokenPayload>
            RemoveSecretOfPersonalAccessTokenAsync(
            [Service] IPersonalAccessTokenService services,
            RemoveSecretPersonalAccessTokenRequest input,
            CancellationToken cancellationToken)
        {
            RemoveSecretPersonalAccessTokenResult result =
                await services
                    .RemoveSecretOfTokenAsync(input.TokenId, input.SecretId, cancellationToken);

            return RemoveSecretPersonalAccessTokenPayload.From(result);
        }
    }
}
