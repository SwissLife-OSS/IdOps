using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace IdOps.Host.Tests
{
    public class StatupTests : IClassFixture<IdOpsTesServer>
    {
        private readonly IdOpsTesServer _testServer;

        public StatupTests(IdOpsTesServer testServer)
        {
           _testServer = testServer;
        }


        [Fact]
        public async Task Start_Root_Redirected()
        {
            //Arrange
            HttpClient client = _testServer.HttpClient;

            //Act
            HttpResponseMessage res = await client.GetAsync("");

            //Assert 
            res.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}
