using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    public class ApiScopeTests : TestHelper
        , IClassFixture<MongoResource>
    {
        public ApiScopeTests(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task ValidRequest()
        {
            // arrange
            ICollection<string> tenants = new List<string>();
            tenants.Add("TestTenant");

            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetApiScopes")
                .AddVariableValue("tenants", tenants)
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertApiScopeIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
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
                .AddVariableValue("tenants", tenants)
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertApiScopeIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
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
                .AddVariableValue("tenants", tenants);

            await InsertApiScopeIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
