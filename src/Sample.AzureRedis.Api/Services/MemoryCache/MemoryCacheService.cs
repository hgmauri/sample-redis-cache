using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Sample.AzureRedis.Api.Services.MemoryCache
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
                
        public Task<T> GetValue<T>(string key)
        {
            return _memoryCache.TryGetValue(key, out T cacheEntry) ? Task.FromResult(cacheEntry) : Task.FromResult<T>(default);
        }
                
        public Task RemoveValue(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        
        public Task SetValue<T>(string key, T value)
        {
            _memoryCache.Set(key, value);
            return Task.CompletedTask;
        }
        
        public Task SetValue<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            _memoryCache.Set(key, value, expirationTimeSpan);
            return Task.CompletedTask;
        }
    }
}
