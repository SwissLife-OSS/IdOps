using System.Threading.Tasks;
using IdOps.Model;

namespace IdOps
{
    public interface ISecretService
    {
        Task<(Secret, string)> CreateSecretAsync(AddSecretRequest request);
    }
}