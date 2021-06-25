using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Sample.AzureRedis.Api.Services.Base;
using StackExchange.Redis;

namespace Sample.AzureRedis.Api.Services.RedisCache
{
    public interface IRedisCacheService : IBaseCacheService
    {
        Task<double> StringIncrementAsync(string key, int value = 1);
        Task<double> StringDecrementAsync(string key, int value = 1);
        Task<long> EnqueueAsync<T>(string key, T value);
        Task<T> DequeueAsync<T>(string key) where T : class;
        Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1) where T : class;
        Task<bool> SetAddAsync<T>(string key, T value);
        Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> values);
        Task<IEnumerable<T>> SetMembersAsync<T>(string key) where T : class;
        Task<bool> SetContainsAsync<T>(string key, T value);
        Task<bool> SortedSetAddAsync(string key, string member, double score);
        Task<long> SortedSetRemoveAsync(string key, IEnumerable<string> members);
        Task<double> SortedSetIncrementAsync(string key, string member, double value);
        Task<double> SortedSetDecrementAsync(string key, string member, double value);
        Task<ConcurrentDictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending);
        Task<ConcurrentDictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key,
                    double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
                    Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);
        Task<ConcurrentDictionary<string, string>> HashGetAsync(string key);
        Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields);
        Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries);
        Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields);
        Task<bool> HashDeleteFieldsAsync(string key, IEnumerable<string> fields);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> GetAllKeys(EndPoint endPoint);
        Task<bool> KeyExistsAsync(string key);
        Task<long> KeyDeleteAsync(IEnumerable<string> keys);
        Task<bool> KeyExpireAsync(string key, TimeSpan? expiry);
        Task<bool> KeyExpireAsync(string key, DateTime? expiry);
        Task<long> PublishAsync(string channel, string msg);
        Task SubscribeAsync(string channel, Action<string, string> handler);
        Task ExecuteBatchAsync(params Action[] operations);
        Task<(bool, object)> LockExecuteAsync(string key, string value, Delegate del, TimeSpan expiry, params object[] args);
        bool LockExecute(string key, string value, Delegate del, out object result, TimeSpan expiry, int timeout = 0, params object[] args);
        bool LockExecute(string key, string value, Action action, TimeSpan expiry, int timeout = 0);
        bool LockExecute<T>(string key, string value, Action<T> action, T arg, TimeSpan expiry, int timeout = 0);
        bool LockExecute<T>(string key, string value, Func<T> func, out T result, TimeSpan expiry, int timeout = 0);
        bool LockExecute<T, TResult>(string key, string value, Func<T, TResult> func, T arg, out TResult result, TimeSpan expiry, int timeout = 0);
    }
}
