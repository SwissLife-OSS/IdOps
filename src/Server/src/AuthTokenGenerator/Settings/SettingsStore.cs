using System.Text.Json;
using IdOps.Abstractions;
using Serilog;

namespace IdOps.Models
{
    public class SettingsStore : ISettingsStore
    {
        private const string AppName = "boost";
        private readonly IUserDataProtector _userDataProtector;

        public SettingsStore(IUserDataProtector userDataProtector)
        {
            _userDataProtector = userDataProtector;
        }

        public async Task SaveAsync<T>(
            T userSettings,
            string fileName,
            string directory = "",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(userSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var path = Path.Combine(GetUserDirectory(directory), $"{fileName}.json");

                await File.WriteAllTextAsync(path, json, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error writing user settings");
                throw;
            }
        }

        public async Task SaveProtectedAsync<T>(
            T data,
            string fileName,
            string directory = "",
            CancellationToken cancellationToken = default)
        {
            var jsonData = JsonSerializer.SerializeToUtf8Bytes(data);
            var encrypted = _userDataProtector.Protect(jsonData);
            var path = Path.Combine(GetUserDirectory(directory), $"{fileName}");

            await File.WriteAllBytesAsync(path, encrypted, cancellationToken);
        }

        public async Task<T?> GetAsync<T>(
            string fileName,
            string directory = "",
            CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(GetUserDirectory(directory), $"{fileName}.json");

            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path, cancellationToken);
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;
        }

        public async Task<T?> GetProtectedAsync<T>(
            string fileName,
            string directory = "",
            CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(GetUserDirectory(directory), $"{fileName}");

            if (File.Exists(path))
            {
                var encryptedData = await File.ReadAllBytesAsync(path, cancellationToken);
                var jsonData = _userDataProtector.UnProtect(encryptedData);

                return JsonSerializer.Deserialize<T>(jsonData);
            }

            return default;
        }

        public Task RemoveAsync(
            string fileName,
            string directory = "",
            CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(GetUserDirectory(directory), fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }

        public static string GetUserDirectory(string directory = "")
        {
            var appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppName, directory);

            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }

            return appData;
        }

        public static string UserSettingsFilePath
            => Path.Combine(
                    GetUserDirectory(),
                    $"{UserSettingsManager.SettingsFileName}.json");
    }
}
