using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using IdOps.Security;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests;

[Collection(TestCollectionNames.GraphQL)]
public class TenantTests : TestHelper
{
    public TenantTests(MongoResource resource) : base(resource)
    {
    }

    [Theory]
    [InlineData(Roles.Admin)]
    [InlineData(Roles.Edit)]
    [InlineData(Roles.Read)]
    public async Task ValidRequest(string role)
    {
        // arrange
        ITestRequestBuilder requestBuilder = TestRequestBuilder
            .New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetTenants")
            .AddUser()
            .AddRole(role);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetWrongTenant_ResultIsEmpty()
    {
        // arrange
        ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetTenants")
            .AddUser()
            .AddRole(Roles.Admin);

        await TestDataBuilder
            .New(Services)
            .SetupWrongTenant()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetTenant_NotAuthenticated()
    {
        // arrange
        ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetTenants")
            .SetAuthenticated(false);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }
}
