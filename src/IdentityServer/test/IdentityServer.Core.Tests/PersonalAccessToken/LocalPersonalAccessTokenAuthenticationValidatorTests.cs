using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using IdOps.IdentityServer;
using Xunit;
using IdOps;
using IdOps.IdentityServer.Hashing;

namespace IdOps.IdentityServer.Tests
{
    public class LocalPersonalAccessTokenAuthenticationValidatorTests
    {
        [Fact]
        public async Task Validate_aValidPat_ShouldSucceed()
        {
            //Arrange
            PersonalAccessTokenMatch aValidPat = GenerateValidPat();

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            var context =
                new ValidationContext<PersonalAccessTokenMatch>(aValidPat);
            context.RootContextData["scopes"] = new List<string> { "api1" };
            context.RootContextData["client"] = "Test.PAT01";
            ValidationResult? validationResult =
                await tokenAuthenticationValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aPatWithUnmatchingScope_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithUnmatchingScope = GenerateValidPat();
            var context =
                new ValidationContext<PersonalAccessTokenMatch>(aPatWithUnmatchingScope);
            context.RootContextData["scopes"] = new List<string> { "NonMatchingScope" };
            context.RootContextData["client"] = "Test.PAT01";

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            ValidationResult? validationResult =
                await tokenAuthenticationValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Single(validationResult.Errors);
        }

        [Fact]
        public async Task Validate_aPatWithUnmatchingClient_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithUnmatchingScope = GenerateValidPat();
            var context =
                new ValidationContext<PersonalAccessTokenMatch>(aPatWithUnmatchingScope);
            context.RootContextData["scopes"] = new List<string> { "api1" };
            context.RootContextData["client"] = "Fail";

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            ValidationResult? validationResult =
                await tokenAuthenticationValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Single(validationResult.Errors);
        }

        [Fact]
        public async Task Validate_aPatWithoutToken_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithoutToken =
                GenerateValidPat(configureHash: x => x with { Token = string.Empty });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(aPatWithoutToken);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Token.Token);
        }

        [Fact]
        public async Task Validate_aPatWithoutUserName_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithoutUserName =
                GenerateValidPat(configureToken: x => x with { UserName = string.Empty });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(aPatWithoutUserName);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Definition.UserName);
        }

        [Fact]
        public async Task Validate_aPatWithIsUsedTrue_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithoutUserName =
                GenerateValidPat(configureHash: x => x with { IsUsed = true });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(aPatWithoutUserName);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Token.IsUsed);
        }

        [Fact]
        public async Task Validate_aPatWithExpiresAtEarlierThanToday_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithExpiresAtEarlierThanToday = GenerateValidPat(x =>
                x with
                {
                    ExpiresAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.Now.AddDays(-2)
                });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(
                    aPatWithExpiresAtEarlierThanToday);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Token.ExpiresAt);
        }

        [Fact]
        public async Task Validate_aPatWithExpiresAtEarlierThanCreatedAt_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithExpiresAtEarlierThanCreatedAt = GenerateValidPat(x =>
                x with { ExpiresAt = DateTime.Now.AddDays(-1), CreatedAt = DateTime.Now });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(
                    aPatWithExpiresAtEarlierThanCreatedAt);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Token.ExpiresAt);
        }

        [Fact]
        public async Task Validate_aPatWithCreatedAtLaterThanToday_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithCreatedAtLaterThanToday =
                GenerateValidPat(
                    x => x with { CreatedAt = DateTime.UtcNow.AddDays(+1) },
                    x => x with { CreatedAt = DateTime.UtcNow.AddDays(+1) });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(
                    aPatWithCreatedAtLaterThanToday);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Definition.CreatedAt);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Token.CreatedAt);
        }

        [Fact]
        public async Task Validate_aPatWithoutSource_ShouldFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatWithoutSource =
                GenerateValidPat(configureToken: x => x with { TokenSource = "" });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            TestValidationResult<PersonalAccessTokenMatch>? validationResult =
                await tokenAuthenticationValidator.TestValidateAsync(aPatWithoutSource);

            //Assert
            Assert.False(validationResult.IsValid);
            validationResult.ShouldHaveValidationErrorFor(pat => pat.Definition.TokenSource);
        }

        [Fact]
        public async Task Validate_aPatThatIsNotOneTime_ShouldNotFail()
        {
            //Arrange
            PersonalAccessTokenMatch aPatThatIsNotOneTime =
                GenerateValidPat(configureToken: x => x with { IsOneTime = false });

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            var context =
                new ValidationContext<PersonalAccessTokenMatch>(aPatThatIsNotOneTime);
            context.RootContextData["scopes"] = new List<string> { "api1" };
            context.RootContextData["client"] = "Test.PAT01";
            ValidationResult validationResult =
                await tokenAuthenticationValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aPatWithAllIssues_ShouldFailAndReportThemAll()
        {
            //Arrange
            //Arrange
            PersonalAccessTokenMatch aPatWithAllIssues = GenerateValidPat(x => x with
                {
                    CreatedAt = DateTime.UtcNow.AddDays(1),
                    ExpiresAt = DateTime.UtcNow.AddDays(-2),
                    IsUsed = true,
                    Token = string.Empty,
                },
                x => x with
                {
                    CreatedAt = DateTime.UtcNow.AddDays(1),
                    IsOneTime = false,
                    UserName = string.Empty,
                    TokenSource = ""
                });

            var context = new ValidationContext<PersonalAccessTokenMatch>(aPatWithAllIssues);

            context.RootContextData["scopes"] = new List<string> { "NonMatchingScope" };
            context.RootContextData["client"] = "NonMatchingClient";

            //Act
            var tokenAuthenticationValidator =
                new LocalPersonalAccessTokenAuthenticationValidator();
            ValidationResult? validationResult =
                await tokenAuthenticationValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Equal(11, validationResult.Errors.Count);
        }

        private PersonalAccessTokenMatch GenerateValidPat(
            Func<IdOpsHashedToken, IdOpsHashedToken>? configureHash = null,
            Func<IdOpsPersonalAccessToken, IdOpsPersonalAccessToken>? configureToken = null)
        {
            var hash = new IdOpsHashedToken(
                Guid.NewGuid(),
                "tokensecret",
                DateTime.UtcNow.AddMonths(12),
                DateTime.UtcNow,
                false);
            hash = configureHash?.Invoke(hash) ?? hash;
            var token = new IdOpsPersonalAccessToken()
            {
                Id = Guid.NewGuid(),
                UserName = "a username",
                HashAlgorithm = SshaHashAlgorithm.Identifier,
                CreatedAt = DateTime.UtcNow,
                Tokens = new List<IdOpsHashedToken>() { hash },
                IsOneTime = true,
                AllowedClients = new[]
                {
                    "Test.PAT01"
                },
                AllowedScopes = new[]
                {
                    "api1"
                },
                TokenSource = Wellknown.PersonalAccessTokens.Source.Local
            };
            token = configureToken?.Invoke(token) ?? token;

            return new PersonalAccessTokenMatch(token, hash);
        }
    }
}
