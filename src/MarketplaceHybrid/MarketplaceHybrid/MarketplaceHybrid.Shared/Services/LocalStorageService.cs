using MarketplaceHybrid.Shared.Services.Interfaces;

namespace MarketplaceHybrid.Shared.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetItemAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task RemoveItemAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task SetItemAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
