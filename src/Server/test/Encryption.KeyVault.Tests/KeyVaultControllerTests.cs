using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using FluentAssertions;
using Moq;
using Xunit;

public class KeyVaultControllerTests
{

    [Theory]
    [InlineData("")]
    [InlineData("Foo")]
    [InlineData("Bar")]
    [InlineData("FooBar")]
    public void Encrypt_Should_ReturnString(string input)
    {
        //Arrange
        byte[] inputAsBytes = Encoding.UTF8.GetBytes(input);
        var encryptResult =
            CryptographyModelFactory.EncryptResult("", inputAsBytes, EncryptionAlgorithm.RsaOaep);

        var cryptographyClientMock = new Mock<CryptographyClient>();
        cryptographyClientMock
            .Setup(client =>
                client.EncryptAsync(It.IsAny<EncryptionAlgorithm>(),
                    It.Is<byte[]>(bytes => bytes.Equals(inputAsBytes)), CancellationToken.None))
            .Returns(Task.FromResult(encryptResult));
        cryptographyClientMock.SetupGet(client => client.KeyId).Returns("");


        var controller = new KeyVaultController(cryptographyClientMock.Object);

        //Act
        var expected = Convert.ToBase64String(encryptResult.Ciphertext);
        var result = controller.Encrypt(input).Result;

        //Assert
        result.Should().Be(expected);
    }


    [Theory]
    [InlineData("")]
    [InlineData("Foo")]
    [InlineData("Bar")]
    [InlineData("FooBar")]
    public void Decrypt_Should_ReturnString(string input)
    {
        //Arrange
        byte[] inputAsByte = Encoding.UTF8.GetBytes(input);
        var encryptResultMock = new Mock<EncryptResult>();

        encryptResultMock.SetupGet(result => result.Ciphertext)
            .Returns(inputAsByte.Reverse().ToArray);

        var cryptographyClientMock = new Mock<CryptographyClient>();
        cryptographyClientMock.Setup<Task<EncryptResult>>(client =>
                client.EncryptAsync(It.IsAny<EncryptionAlgorithm>(),
                    It.Is<byte[]>(bytes => bytes.Equals(inputAsByte)), CancellationToken.None))
            .Returns(new Task<EncryptResult>(() => encryptResultMock.Object));


        var controller = new KeyVaultController(cryptographyClientMock.Object);

        //Act
        var encryptedString = controller.Encrypt(input).Result;
        var decryptedString = controller.Decrypt(encryptedString).Result;

        //Assert
        decryptedString.Should().Be(input);
    }
}
