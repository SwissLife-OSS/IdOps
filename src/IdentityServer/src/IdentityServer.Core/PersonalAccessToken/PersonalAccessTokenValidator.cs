using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Hashing;
using IdOps.IdentityServer.Model;
using IdOps.IdentityServer.Storage;
using Microsoft.Extensions.Logging;

namespace IdOps.IdentityServer
{
    public class PersonalAccessTokenValidator : IPersonalAccessTokenValidator
    {
        private readonly IPersonalAccessTokenRepository _patTokenRepository;
        private readonly IHashAlgorithmResolver _hashAlgorithmResolver;
        private readonly IEventService _eventService;
        private Dictionary<string, IPersonalAccessTokenSource> _tokenSources;

        public PersonalAccessTokenValidator(
            IPersonalAccessTokenRepository repository,
            IHashAlgorithmResolver hashAlgorithmResolver,
            IEnumerable<IPersonalAccessTokenSource> tokenSources,
            IEventService eventService)
        {
            _patTokenRepository = repository;
            _hashAlgorithmResolver = hashAlgorithmResolver;
            _tokenSources = tokenSources.ToDictionary(x => x.Kind);
            _eventService = eventService;
        }

        public async Task<PersonalAccessTokenValidationResult> ValidateAsync(
            PersonalAccessTokenValidationContext context)
        {
            PersonalAccessTokenMatch? maybeToken =
                await TryResolverPersonalAccessToken(context.UserName, context.Token);

            if (!(maybeToken is { } personalAccessToken))
            {
                await PersonalAccessTokenValidationFailedEvent
                    .New(
                        context?.Client?.ClientId ?? "-",
                        context.UserName,
                        "The PAT does not exist for this user",
                        context.RequestedScopes,
                        null)
                    .RaiseAsync(_eventService);

                return PersonalAccessTokenValidationResult.Invalid;
            }

            string tokenSource = personalAccessToken.Definition.TokenSource;
            if (!_tokenSources.TryGetValue(tokenSource, out IPersonalAccessTokenSource? source))
            {
                await PersonalAccessTokenValidationFailedEvent
                    .New(
                        context?.Client?.ClientId ?? "-",
                        context.UserName,
                        $"The TokenSource {tokenSource} could not be determined for user",
                        context.RequestedScopes,
                        null)
                    .RaiseAsync(_eventService);

                return PersonalAccessTokenValidationResult.Invalid;
            }

            PersonalAccessTokenValidationResult result =
                await source.ValidateAsync(context, personalAccessToken);

            if (result.IsValid)
            {
                if (!await ValidateOneTimeAndUpdate(personalAccessToken))
                {
                    await PersonalAccessTokenValidationFailedEvent
                        .New(
                            context?.Client?.ClientId ?? "-",
                            context.UserName,
                            "The PAT is OneTime an was already used",
                            context.RequestedScopes,
                            null)
                        .RaiseAsync(_eventService);

                    return PersonalAccessTokenValidationResult.Invalid;
                }
            }
            else
            {
                await PersonalAccessTokenValidationFailedEvent
                    .New(
                        context?.Client?.ClientId ?? "-",
                        context.UserName,
                        $"PAT validation source: {source.Kind} returned Invalid",
                        context.RequestedScopes,
                        null)
                    .RaiseAsync(_eventService);

                return PersonalAccessTokenValidationResult.Invalid;
            }

            return result;
        }

        private async Task<PersonalAccessTokenMatch?> TryResolverPersonalAccessToken(
            string userName,
            string secret)
        {
            IReadOnlyList<IdOpsPersonalAccessToken> possibleTokens =
                await _patTokenRepository.GetActiveTokensByUserNameAsync(userName,
                    CancellationToken.None);

            foreach (IdOpsPersonalAccessToken tokenDefinition in possibleTokens)
            {
                if (_hashAlgorithmResolver.TryResolve(
                    tokenDefinition.HashAlgorithm,
                    out IHashAlgorithm? validator))
                {
                    foreach (IdOpsHashedToken possibleToken in tokenDefinition.Tokens)
                    {
                        if (validator.Verify(possibleToken.Token, secret))
                        {
                            return new PersonalAccessTokenMatch(tokenDefinition, possibleToken);
                        }
                    }
                }
            }

            return null;
        }

        private async Task<bool> ValidateOneTimeAndUpdate(PersonalAccessTokenMatch patToken)
        {
            if (patToken.Definition.IsOneTime)
            {
                int index = -1;
                for (var i = 0; i < patToken.Definition.Tokens.Count; i++)
                {
                    if (patToken.Definition.Tokens[i].Id == patToken.Token.Id)
                    {
                        index = i;
                    }
                }

                if (index == -1)
                {
                    return false;
                }


                patToken.Definition.Tokens[index] = patToken.Definition.Tokens[index] with
                {
                    IsUsed = true
                };

                await _patTokenRepository.SaveAsync(patToken.Definition, default);
            }

            return true;
        }
    }
}
