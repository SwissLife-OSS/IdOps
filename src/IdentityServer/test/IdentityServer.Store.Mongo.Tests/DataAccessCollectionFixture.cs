using System.Text;
using Squadron;
using Xunit;

namespace IdOps.IdentityServer.Store.Mongo.Tests
{
    public static class TestCollectionNames
    {
        public const string Store = "Store.Mongo";
    }

    [CollectionDefinition(TestCollectionNames.Store)]
    public class DataAccessCollectionFixture : ICollectionFixture<MongoResource>
    { }
}
