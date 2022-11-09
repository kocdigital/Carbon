using Carbon.Caching.Abstractions.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Caching.Abstractions
{
    public static class ICarbonCacheExtensions
    {
        private static CarbonContentSerializationType SerializationType { get; set; }

        public static void SetSerializationType(CarbonContentSerializationType PreferredSerializationType)
        {
            if (PreferredSerializationType == CarbonContentSerializationType.BinaryFormatter)
            {
                Console.WriteLine(PreferredSerializationType + " is obsolete, handle with care or consider changing it!");
                SerializationType = PreferredSerializationType;
            }
            else if (PreferredSerializationType == CarbonContentSerializationType.Json)
            {
                SerializationType = PreferredSerializationType;
            }
            else if (PreferredSerializationType == CarbonContentSerializationType.Protobuf)
            {
                throw new NotImplementedException("Protobuf serialization is not implemented yet!");
            }
        }
        public static void Set<T>(this ICarbonCache instance, string key, T content, TimeSpan period)
        {
            var options = new DistributedCacheEntryOptions();

            if (period != TimeSpan.Zero)
            {
                options = options.SetSlidingExpiration(period);
            }

            instance.Set(key, content.ToByteArray(SerializationType), options);
        }

        public static async Task HashSetAsObject<T>(this ICarbonCache instance, string key, T content)
        {
            var objAsDictionary = JsonSerializer.Deserialize<Dictionary<String, JsonElement>>(JsonSerializer.Serialize(content));
            List<HashEntry> hashEntries = new List<HashEntry>();
            foreach (var entry in objAsDictionary)
            {
                hashEntries.Add(new HashEntry(entry.Key, entry.Value.ToByteArray(CarbonContentSerializationType.Json)));
            }
            await instance.GetDatabase().HashSetAsync(key, hashEntries.ToArray());
        }

        public static async Task<T> HashGetAsObject<T>(this ICarbonCache instance, string key) where T : class
        {
            var values = await instance.GetDatabase().HashGetAllAsync(key);
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            foreach (var k in values)
            {
                keyValuePairs.TryAdd(k.Name, ((byte[])k.Value.Box()).FromByteArray<object>(CarbonContentSerializationType.Json));
            }
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(keyValuePairs));
        }

        public static async Task<T> HashGetAsObject<T>(this ICarbonCache instance, string key, string[] hashfields) where T : class
        {
            RedisValue[] redisValues = new RedisValue[hashfields.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = hashfields[i];
            }
            var values = await instance.GetDatabase().HashGetAsync(key, redisValues);

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            for (int i = 0; i < values.Length; i++)
            {
                keyValuePairs.TryAdd(hashfields[i], ((byte[])values[i].Box()).FromByteArray<object>(CarbonContentSerializationType.Json));
            }

            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(keyValuePairs));
        }

        public static async Task<bool> SetTTL(this ICarbonCache instance, string key, TimeSpan expiryTimeSpan)
        {
            if (expiryTimeSpan == TimeSpan.Zero || expiryTimeSpan == default(TimeSpan))
            {
                throw new NotSupportedException("Expiry Time cannot be set to 0");
            }
            return await instance.GetDatabase().KeyExpireAsync(key, expiryTimeSpan);
        }

        public static async Task HashSet<T>(this ICarbonCache instance, string key, Dictionary<string, T> content)
        {
            List<HashEntry> hashEntries = new List<HashEntry>();
            foreach (var entry in content)
            {
                hashEntries.Add(new HashEntry(entry.Key, entry.Value.ToByteArray(SerializationType)));
            }
            await instance.GetDatabase().HashSetAsync(key, hashEntries.ToArray());
        }

        public static async Task<T> HashGet<T>(this ICarbonCache instance, string key, string hashfield) where T : class
        {
            var value = await instance.GetDatabase().HashGetAsync(key, hashfield);
            return ((byte[])value).FromByteArray<T>(SerializationType);
        }

        public static async Task<Dictionary<string, T>> HashGet<T>(this ICarbonCache instance, string key, string[] hashfields) where T : class
        {
            RedisValue[] redisValues = new RedisValue[hashfields.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = hashfields[i];
            }
            var values = await instance.GetDatabase().HashGetAsync(key, redisValues);
            Dictionary<string, T> keyValuePairs = new Dictionary<string, T>();
            for (int i = 0; i < values.Length; i++)
            {
                keyValuePairs.TryAdd(hashfields[i], ((byte[])values[i]).FromByteArray<T>(SerializationType));
            }

            return keyValuePairs;
        }

        public static async Task<Dictionary<string, T>> HashGet<T>(this ICarbonCache instance, string key) where T : class
        {
            var values = await instance.GetDatabase().HashGetAllAsync(key);
            Dictionary<string, T> keyValuePairs = new Dictionary<string, T>();
            foreach (var k in values)
            {
                keyValuePairs.TryAdd(k.Name, ((byte[])k.Value).FromByteArray<T>(SerializationType));
            }
            return keyValuePairs;
        }

        public static async Task SetAdd<T>(this ICarbonCache instance, string key, T value)
        {
            await instance.GetDatabase().SetAddAsync(key, value.ToByteArray(SerializationType));
        }

        public static async Task SetAdd<T>(this ICarbonCache instance, string key, T[] values)
        {
            RedisValue[] redisValues = new RedisValue[values.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = values[i].ToByteArray(SerializationType);
            }
            await instance.GetDatabase().SetAddAsync(key, redisValues);
        }

        public static async Task SetOverrideAdd<T>(this ICarbonCache instance, string key, T[] values) 
            where T: class
        {
            await instance.RemoveAsync(key);
            await instance.SetAdd<T>(key, values);
        }

        public static async Task SetRemove<T>(this ICarbonCache instance, string key, T[] values)
            where T : class
        {
            RedisValue[] redisValues = new RedisValue[values.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = values[i].ToByteArray(SerializationType);
            }
            await instance.GetDatabase().SetRemoveAsync(key, redisValues);
        }

        public static async Task<List<T>> SetGetMembers<T>(this ICarbonCache instance, string key) 
            where T : class
        {
            var members = await instance.GetDatabase().SetMembersAsync(key);
            return members.Select(k => ((byte[])k).FromByteArray<T>(SerializationType)).ToList();
        }

        public static async Task<List<T>> SetMultiGetMembers<T>(this ICarbonCache instance, SetOperation setOperation, string[] keys)
            where T : class
        {
            List<RedisKey> redisKeys = new List<RedisKey>();
            redisKeys.AddRange(keys.Select(k=> new RedisKey(k)));

            var combinedMembers = await instance.GetDatabase().SetCombineAsync(setOperation, redisKeys.ToArray());
            return combinedMembers.Select(k => ((byte[])k).FromByteArray<T>(SerializationType)).ToList();
        }


        public static void Set<T>(this ICarbonCache instance, string key, T content)
        {
            instance.Set(key, content, TimeSpan.Zero);
        }

        public static T Get<T>(this ICarbonCache instance, string key) where T : class
        {
            var value = instance.Get(key);

            return value?.FromByteArray<T>(SerializationType);
        }

        public static void SetMultiKey<T>(this ICarbonCache instance, IList<string> keys, T content, TimeSpan period)
        {
            foreach (var key in keys)
            {
                instance.Set<T>(key, content, period);
            }
        }
        public static void SetMultiKey<T>(this ICarbonCache instance, IList<string> keys, T content)
        {
            foreach (var key in keys)
            {
                instance.Set<T>(key, content);
            }
        }

        #region IPlatform360Cache - async

        public static async Task<T> GetAsync<T>(this ICarbonCache instance, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var value = await instance.GetAsync(key, token).ConfigureAwait(false);

            return value?.FromByteArray<T>(SerializationType);
        }

        public static async Task<T> GetAsync<T>(this ICarbonCache instance, string key, Func<Task<T>> setMethodIfNotExists, CancellationToken token = default(CancellationToken), TimeSpan timeSpan = default(TimeSpan), bool isSlidingExpiration = true) where T : class
        {
            var value = await instance.GetAsync<T>(key, token).ConfigureAwait(false);
            if (value == null)
            {
                T result = await setMethodIfNotExists();
                await instance.SetAsync(key, result, token, timeSpan, isSlidingExpiration);
                return result;
            }
            return value;
        }


        public static async Task SetAsync<T>(this ICarbonCache instance, string key, T content, CancellationToken token = default(CancellationToken), TimeSpan timeSpan = default(TimeSpan), bool isSlidingExpiration = true)
        {
            if (timeSpan == default(TimeSpan))
                timeSpan = TimeSpan.Zero;
            await instance.SetAsync(key, content, timeSpan, token, isSlidingExpiration).ConfigureAwait(false);
        }

        public static async Task SetAsync<T>(this ICarbonCache instance, string key, T content, TimeSpan period, CancellationToken token = default(CancellationToken), bool isSlidingExpiration = true)
        {
            var options = new DistributedCacheEntryOptions();

            if (period != TimeSpan.Zero)
            {
                if (isSlidingExpiration)
                    options = options.SetSlidingExpiration(period);
                else
                    options = options.SetAbsoluteExpiration(period);
            }

            await instance.SetAsync(key, content.ToByteArray(SerializationType), options, token).ConfigureAwait(false);
        }

        public static async Task SetMultiKeyAsync<T>(this ICarbonCache instance, IList<string> keys, T content)
        {
            foreach (var key in keys)
            {
                await instance.SetAsync<T>(key, content);
            }
        }

        public static async Task SetMultiKeyAsync<T>(this ICarbonCache instance, IList<string> keys, T content, TimeSpan period, CancellationToken token = default(CancellationToken))
        {
            foreach (var key in keys)
            {
                await instance.SetAsync<T>(key, content, period, token);
            }
        }

        public static void RemoveMultiKey(this ICarbonCache instance, IList<string> keys)
        {
            foreach (var key in keys)
            {
                instance.Remove(key);
            }
        }

        public static async Task RemoveMultiKeyAsync(this ICarbonCache instance, IList<string> keys, CancellationToken token = default(CancellationToken))
        {
            foreach (var key in keys)
            {
                await instance.RemoveAsync(key, token);
            }
        }

        #endregion
    }
}
