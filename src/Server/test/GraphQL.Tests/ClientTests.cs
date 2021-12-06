using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    [Collection(TestCollectionNames.GraphQL)]
    public class ClientTests : TestHelper
    {
        public ClientTests(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task ValidRequest()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateClient")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson().MatchSnapshot(x => x.IgnoreFields("**.id"));
        }

        [Fact]
        public async Task CreateClient_ByUserWithoutPermission_IsDenied()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("CreateClient");

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
