using System;
using System.Collections.Generic;
using System.Linq;
using IdOps.IdentityServer.Hashing;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace IdentityServer.Core.Tests
{
    public class HashAlgorithmResolverTests
    {
        [Fact]
        public void Constructor_HashAlgorithmsNull_ThrowException()
        {
            // arrange
            IEnumerable<IHashAlgorithm> encryptors = null!;

            // act
            Exception? ex = Record.Exception(() => new HashAlgorithmResolver(encryptors));

            // assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void Constructor_EmptyResolver_NoException()
        {
            // arrange
            IEnumerable<IHashAlgorithm> encryptors = Enumerable.Empty<IHashAlgorithm>();

            // act
            Exception? ex = Record.Exception(() => new HashAlgorithmResolver(encryptors));

            // assert
            Assert.Null(ex);
        }

        [Fact]
        public void Constructor_WithHashAlgorithms_NoException()
        {
            // arrange
            IEnumerable<IHashAlgorithm> encryptors = Enumerable
                .Empty<IHashAlgorithm>()
                .Append(new Pbkdf2HashAlgorithm())
                .Append(new SshaHashAlgorithm(new PasswordHasher<string>()));

            // act
            Exception? ex = Record.Exception(() => new HashAlgorithmResolver(encryptors));

            // assert
            Assert.Null(ex);
        }

        [Fact]
        public void Resolve_InvalidHashAlgorithmKind_ThrowException()
        {
            // arrange
            IEnumerable<IHashAlgorithm> encryptors = Enumerable
                .Empty<IHashAlgorithm>()
                .Append(new SshaHashAlgorithm(new PasswordHasher<string>()));
            var resolver = new HashAlgorithmResolver(encryptors);

            // act
            // assert
            Assert.False(resolver.TryResolve(Pbkdf2HashAlgorithm.Identifier, out _));
        }

        [Fact]
        public void Resolve_ValidHashAlgorithmKind_ResolveHashAlgorithm()
        {
            // arrange
            IEnumerable<IHashAlgorithm> encryptors = Enumerable
                .Empty<IHashAlgorithm>()
                .Append(new SshaHashAlgorithm(new PasswordHasher<string>()));
            var resolver = new HashAlgorithmResolver(encryptors);

            // act

            // assert
            Assert.True(resolver.TryResolve(SshaHashAlgorithm.Identifier, out _));
        }
    }
}
