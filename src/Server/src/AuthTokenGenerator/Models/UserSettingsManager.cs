using IdOps.Abstractions;

namespace IdOps.Models
{
    public class UserSettingsManager : IUserSettingsManager
    {
        private readonly ISettingsStore _settingsStore;
        internal static readonly string SettingsFileName = "UserSettings";

        public UserSettingsManager(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public async Task<UserSettings> GetAsync(CancellationToken cancellationToken)
        {
            UserSettings? settings = await _settingsStore.GetAsync<UserSettings>(
                SettingsFileName,
                cancellationToken: cancellationToken);

            return settings ?? GetDefaultSettings();
        }

        public async Task SaveWorkRootsAsync(
            IEnumerable<WorkRoot> workRoots,
            CancellationToken cancellationToken)
        {
            UserSettings settings = await GetAsync(cancellationToken);

            settings.WorkRoots = workRoots.ToList();

            await _settingsStore.SaveAsync(
                settings,
                SettingsFileName,
                cancellationToken: cancellationToken);
        }

        public async Task SaveTokenGeneratorSettingsAsync(
            TokenGeneratorSettings tokenGeneratorSettings,
            CancellationToken cancellationToken)
        {
            UserSettings settings = await GetAsync(cancellationToken);

            settings.TokenGenerator = tokenGeneratorSettings;

            await _settingsStore.SaveAsync(
                settings,
                SettingsFileName,
                cancellationToken: cancellationToken);
        }

        public async Task<WorkRoot?> GetWorkRootAsync(
            string? name,
            CancellationToken cancellationToken)
        {
            UserSettings userSettings = await GetAsync(cancellationToken);

            if (name is string)
            {
                WorkRoot? wr = userSettings.WorkRoots.SingleOrDefault(x => x.Name.Equals(
                    name,
                    StringComparison.InvariantCultureIgnoreCase));

                if (wr is { })
                {
                    return wr;
                }
            }

            WorkRoot? defaultWorkRoot = userSettings.WorkRoots.SingleOrDefault(x => x.IsDefault);

            if (defaultWorkRoot is { })
            {
                return defaultWorkRoot;
            }

            return null;
        }

        private UserSettings GetDefaultSettings()
        {
            return new UserSettings
            {
                DefaultShell = DefaultShellService.GetOSDefault()
            };
        }
    }
}
