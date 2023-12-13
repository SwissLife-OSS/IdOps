using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using IdOps.Security;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    [Collection(TestCollectionNames.GraphQL)]
    public class ClientTests : TestHelper
    {
        public ClientTests(MongoResource resource) : base(resource)
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
                .AddRequestFromFile("CreateClient")
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
        [InlineData(Roles.Read)]
        [InlineData(null)]
        public async Task CreateClient_ByUserWithoutPermission_IsDenied(string? role)
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateClient")
                .AddUser()
                .AddRole(role);

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task CreateClient_NotAuthenticated()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateClient")
                .SetAuthenticated(false);

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
