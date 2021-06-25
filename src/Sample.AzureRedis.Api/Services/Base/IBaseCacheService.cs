using System;
using System.Threading.Tasks;

namespace Sample.AzureRedis.Api.Services.Base
{
    public interface IBaseCacheService
    {
        Task<T> StringGetAsync<T>(string key) where T : class;
        Task<bool> StringSetAsync<T>(string key, T value);
        Task<bool> StringSetAsync<T>(string key, T value, TimeSpan expirationTimeSpan);
        Task<bool> KeyDeleteAsync(string key);
    }
}
