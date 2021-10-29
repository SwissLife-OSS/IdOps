using System;
using IdOps.IdentityServer.Hashing;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace IdentityServer.Core.Tests
{
    public class SshaHashAlgorithmTests
    {
        [Fact]
        public void Constructor_PasswordHasherNull_ThrowException()
        {
            // arrange
            IPasswordHasher<string> passwordHasher = null!;

            // act
            Exception? ex = Record.Exception(() => new SshaHashAlgorithm(passwordHasher));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void Constructor_PasswordHasherNotNull_Works()
        {
            // arrange
            IPasswordHasher<string> passwordHasher = new PasswordHasher<string>();

            // act
            Exception? ex = Record.Exception(() => new SshaHashAlgorithm(passwordHasher));

            // assert
            Assert.Null(ex);
        }

        [Fact]
        public void Verify_SecretIsNull_ThrowException()
        {
            // arrange
            var enryptor = new SshaHashAlgorithm(new PasswordHasher<string>());
            string secret = null!;
            string password = "foo";

            // act
            Exception? ex = Record.Exception(() => enryptor.Verify(secret, password));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void Verify_PasswordIsNull_ThrowException()
        {
            // arrange
            var enryptor = new SshaHashAlgorithm(new PasswordHasher<string>());
            string secret = "foo";
            string password = null!;

            // act
            Exception? ex = Record.Exception(() => enryptor.Verify(secret, password));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void Verify_Valid_True()
        {
            // arrange
            var enryptor = new SshaHashAlgorithm(new PasswordHasher<string>());
            string password = "Foo";
            string secret = enryptor.ComputeHash(password);

            // act
            var result = enryptor.Verify(secret, password);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Verify_Valid_False()
        {
            // arrange
            var enryptor = new SshaHashAlgorithm(new PasswordHasher<string>());
            string password = "Foo";
            string secret = enryptor.ComputeHash("Bar");

            // act
            var result = enryptor.Verify(secret, password);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void ComputeHash_PasswordIsNull_ThrowException()
        {
            // arrange
            var enryptor = new SshaHashAlgorithm(new PasswordHasher<string>());
            string password = null!;

            // act
            Exception? ex = Record.Exception(() => enryptor.ComputeHash(password));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }
    }
}
