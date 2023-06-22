using IdOps.Models;

namespace IdOps
{
    public interface ITokenAnalyzer
    {
        TokenModel? Analyze(string token);
    }
}
