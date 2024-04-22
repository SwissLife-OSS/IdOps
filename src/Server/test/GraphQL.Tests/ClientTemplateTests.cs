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
    public class ClientTemplateTests : TestHelper
    {
        public ClientTemplateTests(MongoResource resource) : base(resource)
        {
        }

        [Theory]
        [InlineData(Roles.Admin)]
        [InlineData(Roles.Edit)]
        [InlineData(Roles.Read)]
        public async Task ValidRequest(string role)
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetClientTemplates")
                .AddUser()
                .AddRole(role);

            await TestDataBuilder
                .New(Services)
                .SetupTenant()
                .SetupTemplate()
                .ExecuteAsync();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson()
                .MatchSnapshot();
        }

        [Fact]
        public async Task GetClientTemplates_NotAuthenticated()
        {
            // arrange
            ITestRequestBuilder requestBuilder = TestRequestBuilder.New()
                .AddExecutor(await CreateSchema())
                .AddRequestFromFile("GetClientTemplates")
                .SetAuthenticated(false);

            await TestDataBuilder
                .New(Services)
                .SetupTenant()
                .SetupTemplate()
                .ExecuteAsync();

            // act
            IExecutionResult result = await requestBuilder.ExecuteAsync();

            // assert
            result.ToJson().MatchSnapshot();
        }
    }
}
