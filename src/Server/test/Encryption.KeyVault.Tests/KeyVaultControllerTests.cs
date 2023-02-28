using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using FluentAssertions;
using Moq;
using Xunit;

public class KeyVaultControllerTests
{
    /*
     *  Conversion Chart
     *
     *  Foo;    Rm9v;       {70, 111, 111}
     *  Bar;    QmFy;       {66, 97, 114}
     *  FooBar; Rm9vQmFy;   {70, 111, 111, 66, 97, 114}
     *
     */


    [Theory]
    [InlineData("")]
    [InlineData("Foo")]
    [InlineData("Bar")]
    [InlineData("FooBar")]
    public async void EncryptAsync_Should_ReturnString(string input)
    {
        //Arrange
        var inputAsArray = Encoding.UTF8.GetBytes(input);
        var encryptResult = CryptographyModelFactory.EncryptResult(ciphertext: inputAsArray);


        var cryptoClientMock = new Mock<CryptographyClient>(MockBehavior.Strict);
        cryptoClientMock.Setup(client =>
                client.EncryptAsync(It.IsAny<EncryptionAlgorithm>(), It.IsAny<byte[]>(), default))
            .ReturnsAsync(encryptResult);

        var controller = new KeyVaultController(cryptoClientMock.Object);

        //Act
        string expected = Convert.ToBase64String(inputAsArray);
        string actual = await controller.EncryptAsync(input, CancellationToken.None);

        //Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Rm9v")]
    [InlineData("QmFy")]
    [InlineData("Rm9vQmFy")]
    public async void DecryptAsync_Should_ReturnString(string input)
    {
        //Arrange
        var inputAsArray = Convert.FromBase64String(input);
        var decryptResult = CryptographyModelFactory.DecryptResult(plaintext: inputAsArray);


        var cryptoClientMock = new Mock<CryptographyClient>(MockBehavior.Strict);
        cryptoClientMock.Setup(client =>
                client.DecryptAsync(It.IsAny<EncryptionAlgorithm>(), It.IsAny<byte[]>(), default))
            .ReturnsAsync(decryptResult);

        var controller = new KeyVaultController(cryptoClientMock.Object);

        //Act
        string expected = Encoding.UTF8.GetString(inputAsArray);
        string actual = await controller.DecryptAsync(input, CancellationToken.None);

        //Assert
        actual.Should().Be(expected);
    }
}
