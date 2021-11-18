using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Sample.AzureRedis.Api.Extensions;
using StackExchange.Redis;
using Timer = System.Timers.Timer;

namespace Sample.AzureRedis.Api.Services.RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDatabase;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> StringSetAsync<T>(string key, T value)
        {
            return await _redisDatabase.StringSetAsync(key, value.ToRedisValue());
        }

        public async Task<bool> StringSetAsync<T>(string key, T value, TimeSpan expirationTimeSpan)
        {
            return await _redisDatabase.StringSetAsync(key, value.ToRedisValue(), expirationTimeSpan);
        }

        public async Task<T> StringGetAsync<T>(string key) where T : class
        {
            return (await _redisDatabase.StringGetAsync(key)).ToObject<T>();
        }

        public async Task<double> StringIncrementAsync(string key, int value = 1)
        {
            return await _redisDatabase.StringIncrementAsync(key, value);
        }

        public async Task<double> StringDecrementAsync(string key, int value = 1)
        {
            return await _redisDatabase.StringDecrementAsync(key, value);
        }

        public async Task<long> EnqueueAsync<T>(string key, T value)
        {
            return await _redisDatabase.ListRightPushAsync(key, value.ToRedisValue());
        }

        public async Task<T> DequeueAsync<T>(string key) where T : class
        {
            return (await _redisDatabase.ListLeftPopAsync(key)).ToObject<T>();
        }

        public async Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1) where T : class
        {
            return (await _redisDatabase.ListRangeAsync(key, start, stop)).ToObjects<T>();
        }

        public async Task<bool> SetAddAsync<T>(string key, T value)
        {
            return await _redisDatabase.SetAddAsync(key, value.ToRedisValue());
        }

        public async Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> values)
        {
            return await _redisDatabase.SetRemoveAsync(key, values.ToRedisValues());
        }

        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key) where T : class
        {
            return (await _redisDatabase.SetMembersAsync(key)).ToObjects<T>();
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value)
        {
            return await _redisDatabase.SetContainsAsync(key, value.ToRedisValue());
        }

        public async Task<bool> SortedSetAddAsync(string key, string member, double score)
        {
            return await _redisDatabase.SortedSetAddAsync(key, member, score);
        }

        public async Task<long> SortedSetRemoveAsync(string key, IEnumerable<string> members)
        {
            return await _redisDatabase.SortedSetRemoveAsync(key, members.ToRedisValues());
        }

        public async Task<double> SortedSetIncrementAsync(string key, string member, double value)
        {
            return await _redisDatabase.SortedSetIncrementAsync(key, member, value);
        }

        public async Task<double> SortedSetDecrementAsync(string key, string member, double value)
        {
            return await _redisDatabase.SortedSetDecrementAsync(key, member, value);
        }

        public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            var result = await _redisDatabase.SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
            return result.ToConcurrentDictionary();
        }

        public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
        {
            var result = await _redisDatabase.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take);
            return result.ToConcurrentDictionary();
        }

        public async Task<ConcurrentDictionary<string, string>> HashGetAsync(string key)
        {
            var result = await _redisDatabase.HashGetAllAsync(key);
            return result.ToConcurrentDictionary();
        }

        public async Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields)
        {
            var result = await _redisDatabase.HashGetAsync(key, fields.ToRedisValues());
            return result.ToConcurrentDictionary(fields);
        }

        public async Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries)
        {
            var val = entries.ToHashEntries();
            if (val != null)
                await _redisDatabase.HashSetAsync(key, val);
        }

        public async Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields)
        {
            if (fields == null || !fields.Any())
                return;

            var hs = await HashGetAsync(key);
            foreach (var field in fields)
                hs[field.Key] = field.Value;

            await HashSetAsync(key, hs);
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            return await KeyDeleteAsync(new[] { key }) > 0;
        }

        public async Task<bool> HashDeleteFieldsAsync(string key, IEnumerable<string> fields)
        {
            if (fields == null || !fields.Any())
                return false;

            var success = true;
            foreach (var field in fields)
            {
                if (!await _redisDatabase.HashDeleteAsync(key, field))
                    success = false;
            }

            return success;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _connectionMultiplexer.GetEndPoints().Select(endPoint => _connectionMultiplexer.GetServer(endPoint)).SelectMany(server => server.Keys().ToStrings());
        }

        public IEnumerable<string> GetAllKeys(EndPoint endPoint)
        {
            return _connectionMultiplexer.GetServer(endPoint).Keys().ToStrings();
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _redisDatabase.KeyExistsAsync(key);
        }

        public async Task<long> KeyDeleteAsync(IEnumerable<string> keys)
        {
            return await _redisDatabase.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
        }

        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry)
        {
            return await _redisDatabase.KeyExpireAsync(key, expiry);
        }

        public async Task<bool> KeyExpireAsync(string key, DateTime? expiry)
        {
            return await _redisDatabase.KeyExpireAsync(key, expiry);
        }

        public async Task<long> PublishAsync(string channel, string msg)
        {
            return await _connectionMultiplexer.GetSubscriber().PublishAsync(channel, msg);
        }

        public async Task SubscribeAsync(string channel, Action<string, string> handler)
        {
            await _connectionMultiplexer.GetSubscriber().SubscribeAsync(channel, (chn, msg) => handler(chn, msg));
        }

        public Task ExecuteBatchAsync(params Action[] operations)
        {
            return Task.Run(() =>
            {
                var batch = _redisDatabase.CreateBatch();

                foreach (var operation in operations)
                {
                    operation();
                }

                batch.Execute();
            });
        }

        public async Task<(bool, object)> LockExecuteAsync(string key, string value, Delegate del, TimeSpan expiry, params object[] args)
        {
            if (!await _redisDatabase.LockTakeAsync(key, value, expiry))
                return (false, null);

            try
            {
                return (true, del.DynamicInvoke(args));
            }
            finally
            {
                _redisDatabase.LockRelease(key, value);
            }
        }

        public bool LockExecute(string key, string value, Delegate del, out object result, TimeSpan expiry, int timeout = 0, params object[] args)
        {
            result = null;
            if (!GetLock(key, value, expiry, timeout))
                return false;

            try
            {
                result = del.DynamicInvoke(args);
                return true;
            }
            finally
            {
                _redisDatabase.LockRelease(key, value);
            }
        }

        public bool LockExecute(string key, string value, Action action, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout);
        }

        public bool LockExecute<T>(string key, string value, Action<T> action, T arg, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout, arg);
        }

        public bool LockExecute<T>(string key, string value, Func<T> func, out T result, TimeSpan expiry, int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func();
                return true;
            }
            finally
            {
                _redisDatabase.LockRelease(key, value);
            }
        }

        public bool LockExecute<T, TResult>(string key, string value, Func<T, TResult> func, T arg, out TResult result, TimeSpan expiry, int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func(arg);
                return true;
            }
            finally
            {
                _redisDatabase.LockRelease(key, value);
            }
        }

        private bool GetLock(string key, string value, TimeSpan expiry, int timeout)
        {
            using var waitHandle = new AutoResetEvent(false);
            var timer = new Timer(1000);

            timer.Elapsed += (s, e) =>
            {
                if (!_redisDatabase.LockTake(key, value, expiry))
                    return;
                try
                {
                    waitHandle?.Set();
                    timer?.Stop();
                }
                catch
                {
                    //nothing
                }
            };
            timer.Start();

            if (timeout > 0)
                waitHandle.WaitOne(timeout);
            else
                waitHandle.WaitOne();

            timer.Stop();
            timer.Close();
            timer.Dispose();

            return _redisDatabase.LockQuery(key) == value;
        }
    }
}