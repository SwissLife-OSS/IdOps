using FluentAssertions;
using Xunit;

namespace IdOps.Core.Tests;

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
        var controller = new KeyVaultController(options);

        //Act
        var encryptedString = controller.Encrypt(input).Result;
        var decryptedString = controller.Decrypt(encryptedString).Result;

        //Assert
        decryptedString.Should().Be(input);
    }
}
