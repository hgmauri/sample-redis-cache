using System;
using System.Threading.Tasks;

namespace Sample.AzureRedis.Api.Services.Base
{
    public interface IBaseCacheService
    {
        /// <summary>
        /// Get value with the specify key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetValue<T>(string key);

        /// <summary>
        /// Remove a value from the memory cache
        /// </summary>
        /// <param name="key"></param>
        Task RemoveValue(string key);
        /// <summary>
        /// Set a value into the cache 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task SetValue<T>(string key, T value);
        /// <summary>
        /// Set a value into the cache with an experation timespan
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task SetValue<T>(string key, T value, TimeSpan expirationTimeSpan);
    }
}
