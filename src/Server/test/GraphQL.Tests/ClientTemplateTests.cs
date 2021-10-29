using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using IdOps.Authorization;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    public class ClientTemplateTests : TestHelper
        , IClassFixture<MongoResource>
    {
        public ClientTemplateTests(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task ValidRequest()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetClientTemplates")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertTemplateIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task GetClientTemplates_FromUserWithoutPermission_IsDenied()
        {
            // arrange
            NewServiceForUserWithNoPermission();

            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetClientTemplates")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertTemplateIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task GetClientTemplates_ForWrongTenant_IsDenied()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetClientTemplates")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertTemplateOfWrongTenant();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
