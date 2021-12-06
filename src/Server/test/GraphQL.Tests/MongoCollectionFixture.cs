using Squadron;
using Xunit;

namespace IdOps.GraphQL.Tests
{
    internal static class TestCollectionNames
    {
        public const string GraphQL = "GraphQL.Tests";
    }

    [CollectionDefinition(TestCollectionNames.GraphQL)]
    public class MongoCollectionFixture : ICollectionFixture<MongoResource>
    {
    }
}
