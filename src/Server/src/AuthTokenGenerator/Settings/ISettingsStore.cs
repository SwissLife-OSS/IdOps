namespace IdOps.Abstractions
{
    public interface ISettingsStore
    {
        Task<T?> GetAsync<T>(string fileName, string directory = "", CancellationToken cancellationToken = default);
        Task<T?> GetProtectedAsync<T>(string fileName, string directory = "", CancellationToken cancellationToken = default);
        Task RemoveAsync(string fileName, string directory = "", CancellationToken cancellationToken = default);
        Task SaveAsync<T>(T userSettings, string fileName, string directory = "", CancellationToken cancellationToken = default);
        Task SaveProtectedAsync<T>(T data, string fileName, string directory = "", CancellationToken cancellationToken = default);
    }
}
