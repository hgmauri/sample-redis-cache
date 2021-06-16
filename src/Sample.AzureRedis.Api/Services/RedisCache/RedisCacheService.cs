using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Sample.AzureRedis.Api.Services.RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T> GetValue<T>(string key)
        {
            var redisDatabase = _connectionMultiplexer.GetDatabase();

            var stringValue = await redisDatabase.StringGetAsync(key);

            if (stringValue.IsNullOrEmpty || !stringValue.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(stringValue);
        }

        public async Task RemoveValue(string key)
        {
            var redisDatabase = _connectionMultiplexer.GetDatabase();

            if (await redisDatabase.KeyExistsAsync(key) && !await redisDatabase.KeyDeleteAsync(key))
            {
                throw new Exception($"Cannot delete value with key '{key}' in database");
            }
        }

        public async Task SetValue<T>(string key, T value)
        {
            var redisDatabase = _connectionMultiplexer.GetDatabase();

            if (!await redisDatabase.StringSetAsync(key, JsonSerializer.Serialize(value)))
            {
                throw new Exception("Cannot insert value in database");
            }
        }

        public async Task SetValue<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            var redisDatabase = _connectionMultiplexer.GetDatabase();

            await redisDatabase.StringSetAsync(key, JsonSerializer.Serialize(value), expirationTimeSpan);
        }

        public async Task PublishMessage(string message)
        {
            var redisDatabase = _connectionMultiplexer.GetDatabase();
            await redisDatabase.PublishAsync("channelName", message);
        }
    }
}
