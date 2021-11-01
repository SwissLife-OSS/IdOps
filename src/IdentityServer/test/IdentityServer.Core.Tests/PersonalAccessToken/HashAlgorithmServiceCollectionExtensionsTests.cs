using IdOps.IdentityServer.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdOps.IdentityServer.Tests
{
    public class HashAlgorithmServiceCollectionExtensionsTests
    {
        [Fact]
        public void Resolve_Registered_ResolvePbkdf2()
        {
            // arrange
            ServiceProvider sp = new ServiceCollection()
                .RegisterHashAlgorithms()
                .BuildServiceProvider();

            // act
            bool result = sp
                .GetRequiredService<IHashAlgorithmResolver>()
                .TryResolve(Pbkdf2HashAlgorithm.Identifier, out IHashAlgorithm? algorithm);

            // assert
            Assert.True(result);
            Assert.IsType<Pbkdf2HashAlgorithm>(algorithm);
        }

        [Fact]
        public void Resolve_Registered_ResolveSsha()
        {
            // arrange
            ServiceProvider sp = new ServiceCollection()
                .RegisterHashAlgorithms()
                .BuildServiceProvider();

            // act
            bool result = sp
                .GetRequiredService<IHashAlgorithmResolver>()
                .TryResolve(SshaHashAlgorithm.Identifier, out IHashAlgorithm? algorithm);

            // assert
            Assert.True(result);
            Assert.IsType<SshaHashAlgorithm>(algorithm);
        }
    }
}
