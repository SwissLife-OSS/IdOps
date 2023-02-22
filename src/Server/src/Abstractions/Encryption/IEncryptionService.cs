using System.Threading.Tasks;
public interface IEncryptionService
{

    public string GetEncryptionKeyNameBase64();

    public Task<string> Encrypt(string input);

    public Task<string> Decrypt(string input);



}
