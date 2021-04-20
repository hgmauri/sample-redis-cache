using System;
using System.Threading.Tasks;

namespace Sample.AzureRedis.Api.Services.Base
{
    public interface IBaseCacheService
    {
        Task<T> GetValue<T>(string key);

        Task RemoveValue(string key);

        Task SetValue<T>(string key, T value);

        Task SetValue<T>(string key, T value, TimeSpan expirationTimeSpan);
    }
}
