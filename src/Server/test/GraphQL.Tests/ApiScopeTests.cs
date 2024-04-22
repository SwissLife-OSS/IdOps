using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Security;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests;

[Collection(TestCollectionNames.GraphQL)]
public class ApiScopeTests : TestHelper
{
    public ApiScopeTests(MongoResource resource) : base(resource)
    {
    }

    [Theory]
    [InlineData(Roles.Admin)]
    [InlineData(Roles.Edit)]
    [InlineData(Roles.Read)]
    public async Task ValidRequest(string role)
    {
        // arrange
        ICollection<string> tenants = new List<string>();
        tenants.Add("TestTenant");

        ITestRequestBuilder requestBuilder = TestRequestBuilder
            .New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetApiScopes")
            .AddUser()
            .AddRole(role)
            .AddVariableValue("tenants", tenants);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .SetupApiScope()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetApiScopes_OfWrongTenant_ResultIsEmpty()
    {
        // arrange
        ICollection<string> tenants = new List<string>();
        tenants.Add("Test");

        ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetApiScopes")
            .AddUser()
            .AddRole(Roles.Admin)
            .AddVariableValue("tenants", tenants);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .SetupApiScope()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetApiScopes_ForUserWihtoutPermission_IsDenied()
    {
        // arrange
        ICollection<string> tenants = new List<string>();
        tenants.Add("TestTenant");

        ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetApiScopes")
            .AddUser()
            .AddVariableValue("tenants", tenants);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .SetupApiScope()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetApiScopes_When_NotAuthenticated()
    {
        // arrange
        ICollection<string> tenants = new List<string>();
        tenants.Add("TestTenant");

        ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
            .AddExecutor(await CreateSchema())
            .AddRequestFromFile("GetApiScopes")
            .AddVariableValue("tenants", tenants)
            .SetAuthenticated(false);

        await TestDataBuilder
            .New(Services)
            .SetupTenant()
            .SetupApiScope()
            .ExecuteAsync();

        // act
        IExecutionResult result = await requestBuilder.ExecuteAsync();

        // assert
        result.ToJson().MatchSnapshot();
    }
}
