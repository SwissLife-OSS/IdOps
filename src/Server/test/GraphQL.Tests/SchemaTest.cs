using System.Threading.Tasks;
using HotChocolate.Execution;
using Snapshooter;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    [Collection(TestCollectionNames.GraphQL)]
    public class SchemaTest
        : TestHelper
    {
        public SchemaTest(MongoResource mongoResource) : base(mongoResource)
        {
        }

        [Fact]
        public async Task Schema_IsMatch()
        {
            // arrange
            IRequestExecutor schema = await CreateSchema();

            // act

            // assert
            SnapshotFullName name = new XunitSnapshotFullNameReader().ReadSnapshotFullName();
            schema.Schema
                .Print()
                .MatchSnapshot(new SnapshotFullName("schema.graphql", name.FolderPath));
        }
    }
}
