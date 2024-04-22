using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using IdOps.Security;
using Snapshooter;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    [Collection(TestCollectionNames.GraphQL)]
    public class ApplicationTests
        : TestHelper
    {
        public ApplicationTests(MongoResource resource) : base(resource)
        {
        }

        [Theory]
        [InlineData(Roles.Admin)]
        [InlineData(Roles.Edit)]
        public async Task ValidRequest(string role)
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateApplication")
                .AddUser()
                .AddRole(role);

            await TestDataBuilder
                .New(Services)
                .SetupTenant()
                .ExecuteAsync();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson().MatchSnapshot(x => x.IgnoreFields("**.id"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(Roles.Read)]
        public async Task CreateApplication_ByUserWithoutPermission_IsDenied(string? role)
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddUser()
                .AddRole(role)
                .AddRequestFromFile("CreateApplication");
            
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
        public async Task CreateApplication_NotAuthenticated()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateApplication")
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
}
