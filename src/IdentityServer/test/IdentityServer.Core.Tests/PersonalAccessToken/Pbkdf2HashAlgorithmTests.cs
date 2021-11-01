using System;
using IdOps.IdentityServer.Hashing;
using Xunit;

namespace IdOps.IdentityServer.Tests
{
    public class Pbkdf2HashAlgorithmTests
    {
        [Fact]
        public void Verify_SecretIsNull_ThrowException()
        {
            // arrange
            var enryptor = new Pbkdf2HashAlgorithm();
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
            var enryptor = new Pbkdf2HashAlgorithm();
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
            var enryptor = new Pbkdf2HashAlgorithm();
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
            var enryptor = new Pbkdf2HashAlgorithm();
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
            var enryptor = new Pbkdf2HashAlgorithm();
            string password = null!;

            // act
            Exception? ex = Record.Exception(() => enryptor.ComputeHash(password));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }
    }
}
