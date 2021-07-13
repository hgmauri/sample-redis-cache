using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using StackExchange.Redis;

namespace Sample.AzureRedis.Api.Extensions
{
    public static class StackExchangeRedisExtension
    {
        public static IEnumerable<string> ToStrings(this IEnumerable<RedisKey> keys)
        {
            var redisKeys = keys as RedisKey[] ?? keys.ToArray();
            return !redisKeys.Any() ? null : redisKeys.Select(k => (string)k);
        }

        public static RedisValue ToRedisValue<T>(this T value)
        {
            if (value == null)
                return RedisValue.Null;

            return value is ValueType or string ? value as string : JsonSerializer.Serialize(value);
        }


        public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            return !enumerable.Any() ? null : enumerable.Select(v => v.ToRedisValue()).ToArray();
        }

        public static T ToObject<T>(this RedisValue value) where T : class
        {
            if (value == RedisValue.Null)
                return null;

            return typeof(T) == typeof(string) ? value.ToString() as T : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public static IEnumerable<T> ToObjects<T>(this IEnumerable<RedisValue> values) where T : class
        {
            var redisValues = values as RedisValue[] ?? values.ToArray();
            return !redisValues.Any() ? null : redisValues.Select(v => v.ToObject<T>());
        }

        public static HashEntry[] ToHashEntries(this ConcurrentDictionary<string, string> entries)
        {
            if (entries == null || !entries.Any())
            {
                return null;
            }

            var es = new HashEntry[entries.Count];
            for (var i = 0; i < entries.Count; i++)
            {
                var name = entries.Keys.ElementAt(i);
                var value = entries[name];
                es[i] = new HashEntry(name, value);
            }

            return es;
        }

        public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this IEnumerable<HashEntry> entries)
        {
            var hashEntries = entries as HashEntry[] ?? entries.ToArray();
            if (!hashEntries.Any())
            {
                return null;
            }

            var dict = new ConcurrentDictionary<string, string>();
            foreach (var entry in hashEntries)
            {
                dict[entry.Name] = entry.Value;
            }

            return dict;
        }

        public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this RedisValue[] hashValues,
            IEnumerable<string> fields)
        {
            var enumerable = fields as string[] ?? fields.ToArray();
            if (hashValues == null || !hashValues.Any() || !enumerable.Any())
            {
                return null;
            }

            var dict = new ConcurrentDictionary<string, string>();
            for (var i = 0; i < enumerable.Count(); i++)
            {
                dict[enumerable.ElementAt(i)] = hashValues[i];
            }

            return dict;
        }

        public static ConcurrentDictionary<string, double> ToConcurrentDictionary(this IEnumerable<SortedSetEntry> entries)
        {
            var sortedSetEntries = entries as SortedSetEntry[] ?? entries.ToArray();
            if (!sortedSetEntries.Any())
            {
                return null;
            }

            var dict = new ConcurrentDictionary<string, double>();
            foreach (var entry in sortedSetEntries)
            {
                dict[entry.Element] = entry.Score;
            }

            return dict;
        }
    }
}
