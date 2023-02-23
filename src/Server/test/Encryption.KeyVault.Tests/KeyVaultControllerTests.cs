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
    public void Decrypt_Should_ReturnString(string input)
    {
        //Arrange
        var options = new AzureKeyVaultOptions();
        options.KeyVaultUri = "https://idops-encryptionkeys.vault.azure.net/";
        options.EncryptionKeyName = "TestKey";
        var cryptographyClientMock = new Mock<CryptographyClient>();
        var controller = new KeyVaultController(cryptographyClientMock.Object);

        //Act
        var encryptedString = controller.Encrypt(input).Result;
        var decryptedString = controller.Decrypt(encryptedString).Result;

        //Assert
        decryptedString.Should().Be(input);
    }
}
