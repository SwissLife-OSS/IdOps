using IdOps.Abstractions;

namespace IdOps.Models
{
    public class UserDataAuthTokenStore : IAuthTokenStore
    {
        private readonly ISettingsStore _settingsStore;
        private readonly string TokenPath = "auth_token";

        public UserDataAuthTokenStore(
            ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
        }

        public async Task<TokenStoreModel> GetAsync(string name, CancellationToken cancellationToken)
        {
            TokenStoreModel? tokenData = await _settingsStore.GetProtectedAsync<TokenStoreModel>(
                name,
                TokenPath,
                cancellationToken);

            if (tokenData is null)
            {
                throw new ApplicationException($"No AuthData found with name: {name}");
            }

            return tokenData;
        }

        public async Task StoreAsync(TokenStoreModel model, CancellationToken cancellationToken)
        {
            await _settingsStore.SaveProtectedAsync(
                model,
                model.Name,
                TokenPath,
                cancellationToken);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            return _settingsStore.RemoveAsync(id, TokenPath, cancellationToken);
        }
    }
}
