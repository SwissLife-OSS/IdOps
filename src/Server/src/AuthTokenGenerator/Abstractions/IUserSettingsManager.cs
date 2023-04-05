using IdOps.Models;

namespace IdOps.Abstractions
{
    public interface IUserSettingsManager
    {
        Task<UserSettings> GetAsync(CancellationToken cancellationToken);
        Task<WorkRoot?> GetWorkRootAsync(string? name, CancellationToken cancellationToken);
        Task SaveTokenGeneratorSettingsAsync(TokenGeneratorSettings tokenGeneratorSettings, CancellationToken cancellationToken);
        Task SaveWorkRootsAsync(IEnumerable<WorkRoot> workRoots, CancellationToken cancellationToken);
    }
}
