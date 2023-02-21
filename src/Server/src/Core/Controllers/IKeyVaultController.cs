using System.Threading.Tasks;

namespace IdOps.Controller;

public interface IKeyVaultController
{

    public string GetEncryptionKeyNameBase64();

    public Task<string> Encrypt(string input);

    public Task<string> Decrypt(string input);



}
