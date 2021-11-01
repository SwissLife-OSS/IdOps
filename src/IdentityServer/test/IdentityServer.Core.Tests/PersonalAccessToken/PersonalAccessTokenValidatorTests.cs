using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdOps;
using IdOps.IdentityServer;
using IdOps.IdentityServer.Hashing;
using IdOps.IdentityServer.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace IdentityServer.Core.Tests
{
    public class PersonalAccessTokenValidatorTests
    {
        private static readonly SshaHashAlgorithm
            _hashAlgorithm = new(new PasswordHasher<string>());
        private readonly Mock<IPersonalAccessTokenRepository> _patRepository;
        private IPersonalAccessTokenValidator _patValidator;
        private readonly Mock<IEventService> _eventService;

        public PersonalAccessTokenValidatorTests()
        {
            _eventService = new Mock<IEventService>();
            _patRepository = new Mock<IPersonalAccessTokenRepository>();
            _patValidator = new ServiceCollection()
                .AddSingleton<IPersonalAccessTokenValidator, PersonalAccessTokenValidator>()
                .AddSingleton<IPersonalAccessTokenSource, LocalAccessTokenSource>()
                .RegisterHashAlgorithms()
                .AddSingleton(_patRepository.Object)
                .AddSingleton(_eventService.Object)
                .BuildServiceProvider()
                .GetRequiredService<IPersonalAccessTokenValidator>();
        }

        [Fact]
        public async Task Validate_aValidUserTokenAndScope_ShouldSucceed()
        {
            //Arrange
            SetupMock("Jose", "tokensecret");
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aWrongUserWithValidTokenAndScope_ShouldFail()
        {
            //Arrange
            _patRepository.Setup(
                    x => x.GetActiveTokensByUserNameAsync(
                        It.Is<string>(s => s.Equals("Philippe")),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new IdOpsPersonalAccessToken[0]);
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api"
                },
                string.Empty,
                "Philippe",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aWrongScopeWithValidUserAndToken_ShouldFail()
        {
            //Arrange
            SetupMock("Jose", "tokensecret");
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "wrongscopehere"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aValidPatWithIsOneTimeSet_InvokesSaveAsync()
        {
            //Arrange
            SetupMock("Jose", "tokensecret", x => x with { IsOneTime = true });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            _patRepository.Verify(m => m.SaveAsync(
                It.Is<IdOpsPersonalAccessToken>(x => x.Tokens[0].IsUsed),
                default
            ));
        }

        [Fact]
        public async Task Validate_AdsvumValidationFailed_ShouldNotSucceed()
        {
            //Arrange
            SetupMock("Jose",
                "tokensecret",
                configureToken: x => x with { ExpiresAt = DateTime.UtcNow.AddDays(-1) });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Equal("invalid_request", validationResult.GrantValidationResult.Error);
        }

        [Fact]
        public async Task Validate_aValidNonePat_Invalid()
        {
            //Arrange
            SetupMock(
                "Jose",
                "tokensecret",
                x => x with { TokenSource = "" });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Equal("invalid_request", validationResult.GrantValidationResult.Error);
        }

        [Fact]
        public async Task Validate_aValidLocalUserTokenAndScope_ShouldSucceed()
        {
            //Arrange
            SetupMock("Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = true,
                    TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                    AllowedScopes = new[]
                    {
                        "api1"
                    }
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aLocalUserMissingScope_ShouldNotSucceed()
        {
            //Arrange
            SetupMock("Jose",
                "tokensecret",
                x =>
                    x with
                    {
                        IsOneTime = true,
                        TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                        AllowedScopes = new[]
                        {
                            "api2"
                        }
                    });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.Equal("invalid_request", validationResult.GrantValidationResult.Error);
        }

        [Fact]
        public async Task Validate_aLocalUserNotOneTime_ShouldNotSucceed()
        {
            //Arrange
            SetupMock("Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = false,
                    TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                    AllowedScopes = new[]
                    {
                        "api1"
                    }
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_aValidLocalPatExtraClaims_ContainsClaims()
        {
            //Arrange
            SetupMock(
                "Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = true,
                    TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                    ClaimExtensions = x.ClaimExtensions
                        .Append(new ClaimExtension() { Type = "Foo", Value = "Bar" })
                        .Append(new ClaimExtension() { Type = "Foo2", Value = "Bar2" })
                        .ToArray()
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
            validationResult.AssertClaim("Foo", "Bar");
            validationResult.AssertClaim("Foo2", "Bar2");
        }

        [Fact]
        public async Task Validate_aLocalPatTakeSubjectFromClaims_ContainsClaims()
        {
            //Arrange
            SetupMock(
                "Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = true,
                    TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                    ClaimExtensions = x.ClaimExtensions
                        .Append(new ClaimExtension() { Type = "sub", Value = "customsub" })
                        .Append(new ClaimExtension() { Type = "Foo2", Value = "Bar2" })
                        .ToArray()
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
            validationResult.AssertClaim("sub", "customsub");
            validationResult.AssertClaim("Foo2", "Bar2");
        }

        [Fact]
        public async Task Validate_aLocalPatTakeSubjectFromUserName_ContainsSubClaims()
        {
            //Arrange
            SetupMock(
                "Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = true,
                    TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                    ClaimExtensions = x.ClaimExtensions
                        .Append(new ClaimExtension() { Type = "Foo2", Value = "Bar2" })
                        .ToArray()
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            Assert.True(validationResult.IsValid);
            validationResult.AssertClaim("sub", "Jose");
        }

        [Fact]
        public async Task Validate_aValidLocalPatWithIsOneTimeSet_InvokesSaveAsync()
        {
            //Arrange
            SetupMock(
                "Jose",
                "tokensecret",
                x => x with
                {
                    IsOneTime = true, TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                });
            PersonalAccessTokenValidationContext context = new(
                new[]
                {
                    "api1"
                },
                string.Empty,
                "Jose",
                new Client { ClientId = "Test.PAT01" },
                null!,
                "sid",
                "tokensecret"
            );

            //Act
            PersonalAccessTokenValidationResult validationResult =
                await _patValidator.ValidateAsync(context);

            //Assert
            _patRepository.Verify(m => m.SaveAsync(
                It.Is<IdOpsPersonalAccessToken>(x => x.Tokens[0].IsUsed),
                default
            ));
        }

        private void SetupMock(
            string username,
            string tokensecret,
            Func<IdOpsPersonalAccessToken, IdOpsPersonalAccessToken>? configure = null,
            Func<IdOpsHashedToken, IdOpsHashedToken>? configureToken = null)
        {
            _patRepository.Setup(
                    x => x.GetActiveTokensByUserNameAsync(
                        It.Is<string>(s => s.Equals(username)),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    IdOpsHashedToken hashedToken = new(Guid.NewGuid(),
                        _hashAlgorithm.ComputeHash(tokensecret),
                        DateTime.UtcNow.AddMonths(12),
                        DateTime.UtcNow.AddDays(-1),
                        false
                    );
                    hashedToken = configureToken?.Invoke(hashedToken) ?? hashedToken;

                    var token = new IdOpsPersonalAccessToken()
                    {
                        AllowedClients = new[]
                        {
                            "Test.PAT01"
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        IsOneTime = false,
                        UserName = username,
                        HashAlgorithm = SshaHashAlgorithm.Identifier,
                        Tokens = new List<IdOpsHashedToken> { hashedToken },
                        TokenSource = Wellknown.PersonalAccessTokens.Source.Local,
                        AllowedScopes = new List<string> { "api1" }
                    };

                    token = configure?.Invoke(token) ?? token;

                    return new[]
                    {
                        token
                    };
                });
        }
    }

    public static class TestingClaimExtensions
    {
        public static void AssertClaim(
            this PersonalAccessTokenValidationResult result,
            string type,
            string value)
        {
            Assert.True(
                result.GrantValidationResult.Subject.Claims.Any(
                    x => x.Type == type && x.Value == value),
                $"No Claim of type {type} and with value {value} found");
        }
    }
}
