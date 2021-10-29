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
    public class TenantTests : TestHelper
        , IClassFixture<MongoResource>
    {
        public TenantTests(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task ValidRequest()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetTenants")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertTenantIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task GetWrongTenant_ResultIsEmpty()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetTenants")
                .AddScope(AuthorizationPolicies.Names.ApiAccess);

            await InsertWrongTenantIntoDB();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task GetTenant_ByUserWithoutPermission_IsDenied()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetTenants");

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }
    }
}
