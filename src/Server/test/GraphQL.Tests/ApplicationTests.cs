using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using Snapshooter;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    public class ApplicationTests
        : TestHelper
        , IClassFixture<MongoResource>
    {
        public ApplicationTests(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task ValidRequest()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateApplication")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson().MatchSnapshot(x => x.IgnoreFields("**.id"));
        }

        [Fact]
        public async Task CreateApplication_ByUserWithoutPermission_IsDenied()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateApplication");

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
