using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;
using Squadron;
using Xunit;
using Microsoft.Extensions.Configuration;
using IdOps.Api;

namespace IdOps.Host.Tests
{
    public class IdOpsTesServer : IAsyncLifetime
    {
        public HttpClient? HttpClient { get; private set; }

        public MongoResource? MongoResource { get; private set; }
        public IMongoDatabase Database { get; private set; }

        public async Task DisposeAsync()
        {
            await MongoResource?.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            MongoResource = new MongoResource();
            await MongoResource.InitializeAsync();

            Database = MongoResource.CreateDatabase();

            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(builder =>
                {

                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["IdOps-Server:Storage:Database:ConnectionString"] =
                        MongoResource.ConnectionString,
                        ["IdOps-Server:Storage:Database:DatabaseName"] =
                        Database.DatabaseNamespace.DatabaseName
                    });
                })
                .ConfigureTestServices(services =>
                {

                })
                .UseStartup<Startup>();

            var server = new TestServer(hostBuilder);

            HttpClient = server.CreateClient();
        }
    }
}
