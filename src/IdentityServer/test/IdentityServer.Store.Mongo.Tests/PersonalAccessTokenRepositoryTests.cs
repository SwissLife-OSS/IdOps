using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdOps.IdentityServer.Model;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Storage.Mongo.Tests;

[Collection(TestCollectionNames.Store)]
public class PersonalAccessTokenRepositoryTests : RepositoryTest
{
    public PersonalAccessTokenRepositoryTests(MongoResource mongoResource) : base(mongoResource) 
    { }


    [Fact]
    public async Task GetActiveTokensByUserNameAsync_UserNameHasDifferentCase_UserNameFound()
    {
        // Arrange
        var userName = "UserOne";
        var differentCaseUserName = "userone";
        var personalAccessToken = new IdOpsPersonalAccessToken
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Tokens = new List<IdOpsHashedToken>
            {
                new(
                    Guid.NewGuid(),
                    "foobar",
                    DateTime.Parse("2031.01.02"),
                    DateTime.Parse("2001.01.02"),
                    false)
            }
        };
        var repository = new PersonalAccessTokenRepository(DbContext);

        // Act
        await repository.CreateAsync(personalAccessToken, default);
        var activeTokens =
            await repository.GetActiveTokensByUserNameAsync(differentCaseUserName, default);

        // Assert
        activeTokens.Should().NotBeNullOrEmpty();
        activeTokens.Should().ContainSingle();
        activeTokens[0].UserName.Should().Be(userName);
    }
}
