using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Sample.AzureRedis.Api.Services.MemoryCache
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T> StringGetAsync<T>(string key) where T : class
        {
            return _memoryCache.TryGetValue(key, out T cacheEntry) ? Task.FromResult(cacheEntry) : Task.FromResult<T>(default);
        }

        public Task<bool> StringSetAsync<T>(string key, T value)
        {
            _memoryCache.Set(key, value);
            return Task.FromResult(true);
        }

        public Task<bool> StringSetAsync<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            _memoryCache.Set(key, value, expirationTimeSpan);
            return Task.FromResult(true);
        }

        public Task<bool> KeyDeleteAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.FromResult(true);
        }
    }
}
