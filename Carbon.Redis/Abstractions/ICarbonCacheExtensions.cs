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
        private static readonly string HashData = "data";
        /// <summary>
        /// Sets the serialization type for all these extensions
        /// </summary>
        /// <param name="PreferredSerializationType">BinaryFormatter (Obsolete), Json, Protobuf (Not implemented Yet)</param>
        /// <exception cref="NotImplementedException">Protobuf serialization is not implemented yet!</exception>
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

        /// <summary>
        /// Simple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <param name="period">Set Sliding TTL</param>
        public static void Set<T>(this ICarbonCache instance, string key, T content, TimeSpan period)
        {
            var options = new DistributedCacheEntryOptions();

            if (period != TimeSpan.Zero)
            {
                options = options.SetSlidingExpiration(period);
            }

            instance.Set(key, content.ToByteArray(SerializationType), options);
        }

        /// <summary>
        /// Converts your flat object into hash key-value pairs within a single redis hash key, you can retrieve single or multiple fields later
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> HashSetAsObject<T>(this ICarbonCache instance, string key, T content)
        {

            var objAsDictionary = JsonSerializer.Deserialize<Dictionary<String, JsonElement>>(JsonSerializer.Serialize(content));
            List<HashEntry> hashEntries = new List<HashEntry>();
            foreach (var entry in objAsDictionary)
            {
                hashEntries.Add(new HashEntry(entry.Key, entry.Value.ToByteArray(CarbonContentSerializationType.Json)));
            }
            await instance.GetDatabase().HashSetAsync(key.ToRedisInstanceKey(instance), hashEntries.ToArray());
            return (instance, key);
        }

        /// <summary>
        /// Gets your entire object that is converted into hash key-value pairs within a single redis hash key.
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <returns>Your entire object</returns>
        public static async Task<T> HashGetAsObject<T>(this ICarbonCache instance, string key) where T : class
        {
            var values = await instance.GetDatabase().HashGetAllAsync(key.ToRedisInstanceKey(instance), CommandFlags.PreferReplica);
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            foreach (var k in values)
            {
                keyValuePairs.TryAdd(k.Name, ((byte[])k.Value.Box()).FromByteArray<object>(CarbonContentSerializationType.Json));
            }
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(keyValuePairs));
        }

        /// <summary>
        /// Gets your entire object with the given fields that is converted into hash key-value pairs within a single redis hash key.
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="hashfields">Pass which fields to be returned, not provided ones will be null or default</param>
        /// <returns>Your entire object filled for the given fields</returns>
        public static async Task<T> HashGetAsObject<T>(this ICarbonCache instance, string key, string[] hashfields) where T : class
        {
            RedisValue[] redisValues = new RedisValue[hashfields.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = hashfields[i];
            }
            var values = await instance.GetDatabase().HashGetAsync(key.ToRedisInstanceKey(instance), redisValues, CommandFlags.PreferReplica);

            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            for (int i = 0; i < values.Length; i++)
            {
                keyValuePairs.TryAdd(hashfields[i], ((byte[])values[i].Box()).FromByteArray<object>(CarbonContentSerializationType.Json));
            }

            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(keyValuePairs));
        }

        /// <summary>
        /// Gets your entire object with the given fields that is converted into hash key-value pairs within a single redis hash key, if key does not exist, it tries to fill with the given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="setMethodIfNotExists">Execute if this key does not exist</param>
        /// <returns>Your entire object</returns>
        public static async Task<T> HashGetAsObject<T>(this ICarbonCache instance, string key, Func<Task<T>> setMethodIfNotExists) where T : class
        {
            var value = await instance.HashGetAsObject<T>(key);

            if(value == default)
            {
                T result = await setMethodIfNotExists();
                await instance.HashGetAsObject<T>(key);
                return result;
            }

            return value;
        }

        /// <summary>
        /// Set an expiration time for your redis key (TTL) Time-To-Live
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="expiryTimeSpan">Absolute Time Span starting as of now</param>
        /// <returns>Successful or Failed response directly from Redis</returns>
        /// <exception cref="NotSupportedException">Expiry Time cannot be set to 0</exception>
        public static async Task<bool> SetTTL(this ICarbonCache instance, string key, TimeSpan expiryTimeSpan)
        {
            if (expiryTimeSpan == TimeSpan.Zero || expiryTimeSpan == default(TimeSpan))
            {
                throw new NotSupportedException("Expiry Time cannot be set to 0");
            }
            return await instance.GetDatabase().KeyExpireAsync(key.ToRedisInstanceKey(instance), expiryTimeSpan);
        }

        /// <summary>
        /// Set an expiration time for your redis key (TTL) Time-To-Live
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="expiryTimeSpan">Absolute Time Span starting as of now</param>
        /// <returns>Successful or Failed response directly from Redis</returns>
        /// <exception cref="NotSupportedException">Expiry Time cannot be set to 0</exception>
        public static async Task<bool> SetTTL(this Task<(ICarbonCache, string)> instanceWithKey, TimeSpan expiryTimeSpan)
        {
            var awaitedInstanceWithKey = await instanceWithKey;
            if (expiryTimeSpan == TimeSpan.Zero || expiryTimeSpan == default(TimeSpan))
            {
                throw new NotSupportedException("Expiry Time cannot be set to 0");
            }
            return await awaitedInstanceWithKey.Item1.GetDatabase().KeyExpireAsync(awaitedInstanceWithKey.Item1.GetInstanceName() + awaitedInstanceWithKey.Item2, expiryTimeSpan);
        }

        /// <summary>
        /// Simple set a hash in redis with a single object where your content will be a single hash field in the hash key
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> HashSingleSet<T>(this ICarbonCache instance, string key, T content)
        {
            HashEntry hashEntry = new HashEntry(HashData, content.ToByteArray(SerializationType));

            await instance.GetDatabase().HashSetAsync(key.ToRedisInstanceKey(instance), new HashEntry[] { hashEntry });
            return (instance, key);
        }

        /// <summary>
        /// Simple gets your entire object as single that is converted into single hash field within that redis hash key.
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from given field/dictionary key</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="hashfield">The field/dictionary key's value you want to retrieve</param>
        /// <returns>A single value for the given hash field</returns>
        public static async Task<T> HashSingleGet<T>(this ICarbonCache instance, string key) where T : class
        {
            var value = await instance.GetDatabase().HashGetAsync(key.ToRedisInstanceKey(instance), HashData, CommandFlags.PreferReplica);
            return ((byte[])value).FromByteArray<T>(SerializationType);
        }

        /// <summary>
        /// Simple gets your entire object as single that is converted into single hash field within that redis hash key, if key does not exist, it tries to fill with the given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="setMethodIfNotExists">Execute if this key does not exist</param>
        /// <returns></returns>
        public static async Task<T> HashSingleGet<T>(this ICarbonCache instance, string key, Func<Task<T>> setMethodIfNotExists) where T : class
        {
            var value = await instance.HashSingleGet<T>(key);

            if (value == default)
            {
                T result = await setMethodIfNotExists();
                await instance.HashSingleSet<T>(key, result);
                return result;
            }

            return value;
        }

        /// <summary>
        /// Set a hash in redis with the given dictionary where each key will be another hash field in the same hash key
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> HashSet<T>(this ICarbonCache instance, string key, Dictionary<string, T> content)
        {
            List<HashEntry> hashEntries = new List<HashEntry>();
            foreach (var entry in content)
            {
                hashEntries.Add(new HashEntry(entry.Key, entry.Value.ToByteArray(SerializationType)));
            }
            await instance.GetDatabase().HashSetAsync(key.ToRedisInstanceKey(instance), hashEntries.ToArray());
            return (instance, key);
        }

        /// <summary>
        /// Gets your entire object with the given fields that is converted into hash key-value pairs within a single redis hash key.
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from given field/dictionary key</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="hashfield">The field/dictionary key's value you want to retrieve</param>
        /// <returns>A single value for the given hash field</returns>
        public static async Task<T> HashGet<T>(this ICarbonCache instance, string key, string hashfield) where T : class
        {
            var value = await instance.GetDatabase().HashGetAsync(key.ToRedisInstanceKey(instance), hashfield, CommandFlags.PreferReplica);
            return ((byte[])value).FromByteArray<T>(SerializationType);
        }

        /// <summary>
        /// Gets your entire information in a dictionary with the given fields within a single redis hash key.
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from given field/dictionary key, use object if all the types may differ, then convert it later</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="hashfield">The field/dictionary keys' values you want to retrieve</param>
        /// <returns>Dictionary of the requested hash fields</returns>
        public static async Task<Dictionary<string, T>> HashGet<T>(this ICarbonCache instance, string key, string[] hashfields) where T : class
        {
            RedisValue[] redisValues = new RedisValue[hashfields.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = hashfields[i];
            }
            var values = await instance.GetDatabase().HashGetAsync(key.ToRedisInstanceKey(instance), redisValues, CommandFlags.PreferReplica);
            Dictionary<string, T> keyValuePairs = new Dictionary<string, T>();
            for (int i = 0; i < values.Length; i++)
            {
                keyValuePairs.TryAdd(hashfields[i], ((byte[])values[i]).FromByteArray<T>(SerializationType));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Gets your entire information in a dictionary for all fields within a single redis hash key.
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from given field/dictionary key, use object if all the types may differ, then convert it later</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <returns>Dictionary of the all hash fields</returns>
        public static async Task<Dictionary<string, T>> HashGet<T>(this ICarbonCache instance, string key) where T : class
        {
            var values = await instance.GetDatabase().HashGetAllAsync(key.ToRedisInstanceKey(instance), CommandFlags.PreferReplica);
            Dictionary<string, T> keyValuePairs = new Dictionary<string, T>();
            foreach (var k in values)
            {
                keyValuePairs.TryAdd(k.Name, ((byte[])k.Value).FromByteArray<T>(SerializationType));
            }
            return keyValuePairs;
        }


        /// <summary>
        /// Gets your entire information in a dictionary for all fields within a single redis hash key, if key does not exist, it tries to fill with the given function
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from given field/dictionary key, use object if all the types may differ, then convert it later</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="setMethodIfNotExists">Execute if this key does not exist</param>
        /// <returns>Dictionary of the all hash fields</returns>
        public static async Task<Dictionary<string, T>> HashGet<T>(this ICarbonCache instance, string key, Func<Task<Dictionary<string, T>>> setMethodIfNotExists) where T : class
        {
            var values = await instance.HashGet<T>(key);

            if (values == default || !values.Any())
            {
                Dictionary<string, T> result = await setMethodIfNotExists();
                await instance.HashSet(key, result);
                return result;
            }
            return values;
        }


        /// <summary>
        /// Add an object to a Redis Set within the given key
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="value">Your content to be stored in redis set</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> SetAdd<T>(this ICarbonCache instance, string key, T value)
        {
            await instance.GetDatabase().SetAddAsync(key.ToRedisInstanceKey(instance), value.ToByteArray(SerializationType));
            return (instance, key);
        }

        /// <summary>
        /// Add multiple objects at a time to a Redis Set within the given key
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="values">Your content list to be stored in redis set</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> SetAdd<T>(this ICarbonCache instance, string key, T[] values)
        {
            RedisValue[] redisValues = new RedisValue[values.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = values[i].ToByteArray(SerializationType);
            }
            await instance.GetDatabase().SetAddAsync(key.ToRedisInstanceKey(instance), redisValues);
            return (instance, key);
        }

        /// <summary>
        /// Removes the given Redis Set and then recreates with the given values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="values">Your content list to be stored in redis set</param>
        /// <returns></returns>
        public static async Task<(ICarbonCache, string)> SetOverrideAdd<T>(this ICarbonCache instance, string key, T[] values)
            where T : class
        {
            await instance.RemoveAsync(key.ToRedisInstanceKey(instance));
            await instance.SetAdd<T>(key.ToRedisInstanceKey(instance), values);
            return (instance, key);
        }

        /// <summary>
        /// Removes values in a Redis Set
        /// </summary>
        /// <typeparam name="T">Type of set value to be removed</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="values">Values to be removed from the set</param>
        /// <returns></returns>
        public static async Task SetRemove<T>(this ICarbonCache instance, string key, T[] values)
            where T : class
        {
            RedisValue[] redisValues = new RedisValue[values.Length];
            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = values[i].ToByteArray(SerializationType);
            }
            await instance.GetDatabase().SetRemoveAsync(key.ToRedisInstanceKey(instance), redisValues);
        }

        /// <summary>
        /// Get all the members in Redis Set
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from set, use object if all the types may differ, then convert it later</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <returns>List of your members</returns>
        public static async Task<List<T>> SetGetMembers<T>(this ICarbonCache instance, string key)
            where T : class
        {
            var members = await instance.GetDatabase().SetMembersAsync(key.ToRedisInstanceKey(instance), CommandFlags.PreferReplica);
            return members.Select(k => ((byte[])k).FromByteArray<T>(SerializationType)).ToList();
        }

        /// <summary>
        /// Get Members by applying set operations such as Union, Intersect, Difference for multiple keys
        /// </summary>
        /// <typeparam name="T">The value type that you want to retrieve from set, use object if all the types may differ, then convert it later</typeparam>
        /// <param name="instance"></param>
        /// <param name="setOperation">Union, Intersect, Difference</param>
        /// <param name="keys">Which keys you want to apply operations against</param>
        /// <returns>List of found members after the operation</returns>
        public static async Task<List<T>> SetMultiGetMembers<T>(this ICarbonCache instance, SetOperation setOperation, string[] keys)
            where T : class
        {
            List<RedisKey> redisKeys = new List<RedisKey>();
            redisKeys.AddRange(keys.ToRedisInstanceKey(instance).Select(k => new RedisKey(k)));

            var combinedMembers = await instance.GetDatabase().SetCombineAsync(setOperation, redisKeys.ToArray(), CommandFlags.PreferReplica);
            return combinedMembers.Select(k => ((byte[])k).FromByteArray<T>(SerializationType)).ToList();
        }


        /// <summary>
        /// Simple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed. No TTL - Persistant Key
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        public static void Set<T>(this ICarbonCache instance, string key, T content)
        {
            instance.Set(key, content, TimeSpan.Zero);
        }

        /// <summary>
        /// Simple get operation which is inherited from IDistributedCache interface, all the get logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key to be retrieved</param>
        public static T Get<T>(this ICarbonCache instance, string key) where T : class
        {
            var value = instance.Get(key);

            return value?.FromByteArray<T>(SerializationType);
        }

        /// <summary>
        /// Simple multiple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// Set the same object into multiple keys
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="keys">Redis Keys</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <param name="period">Set Sliding TTL for all keys</param>      
        public static void SetMultiKey<T>(this ICarbonCache instance, IList<string> keys, T content, TimeSpan period)
        {
            foreach (var key in keys)
            {
                instance.Set<T>(key, content, period);
            }
        }

        /// <summary>
        /// Simple multiple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// Set the same object into multiple keys without any TTL - Persistent key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="keys">Redis Keys</param>
        /// <param name="content">Content to be serialized into binary</param>
        public static void SetMultiKey<T>(this ICarbonCache instance, IList<string> keys, T content)
        {
            foreach (var key in keys)
            {
                instance.Set<T>(key, content);
            }
        }

        #region IPlatform360Cache - async

        /// <summary>
        /// Simple awaitable get async operation which is inherited from IDistributedCache interface, all the get logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this ICarbonCache instance, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var value = await instance.GetAsync(key, token).ConfigureAwait(false);

            return value?.FromByteArray<T>(SerializationType);
        }

        /// <summary>
        /// Simple awaitable get async operation which is inherited from IDistributedCache interface, all the get logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// This method gets a key from redis. If the key does not exist, it executes the method you provided, and the return value that is returned from this method will be set
        /// as a Simple Redis Key with a given TTL.
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="setMethodIfNotExists">Execute if this key does not exist</param>
        /// <param name="token"></param>
        /// <param name="timeSpan">TTL time span</param>
        /// <param name="isSlidingExpiration">Is sliding or absolute</param>
        /// <returns></returns>
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


        /// <summary>
        /// Simple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed with TTL and absolute or sliding expiration time as you wish
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <param name="token"></param>
        /// <param name="timeSpan">TTL time span</param>
        /// <param name="isSlidingExpiration">Is sliding or absolute</param>
        /// <returns></returns>
        public static async Task SetAsync<T>(this ICarbonCache instance, string key, T content, CancellationToken token = default(CancellationToken), TimeSpan timeSpan = default(TimeSpan), bool isSlidingExpiration = true)
        {
            if (timeSpan == default(TimeSpan))
                timeSpan = TimeSpan.Zero;
            await instance.SetAsync(key, content, timeSpan, token, isSlidingExpiration).ConfigureAwait(false);
        }

        /// <summary>
        /// Simple set operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed with TTL and absolute or sliding expiration time as you wish
        /// </summary>
        /// <typeparam name="T">Any object type that is serializable</typeparam>
        /// <param name="instance"></param>
        /// <param name="key">Redis Key</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <param name="token"></param>
        /// <param name="period">TTL time span</param>
        /// <param name="isSlidingExpiration">Is sliding or absolute</param>
        /// <returns></returns>
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

        /// <summary>
        /// Simple multiple set async operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// Set the same object into multiple keys without any TTL - Persistent key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="keys">Redis Keys</param>
        /// <param name="content">Content to be serialized into binary</param>
        public static async Task SetMultiKeyAsync<T>(this ICarbonCache instance, IList<string> keys, T content)
        {
            foreach (var key in keys)
            {
                await instance.SetAsync<T>(key, content);
            }
        }

        /// <summary>
        /// Simple multiple set async operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// Set the same object into multiple keys without any TTL - Persistent key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="keys">Redis Keys</param>
        /// <param name="content">Content to be serialized into binary</param>
        /// <param name="period">TTL</param>
        public static async Task SetMultiKeyAsync<T>(this ICarbonCache instance, IList<string> keys, T content, TimeSpan period, CancellationToken token = default(CancellationToken))
        {
            foreach (var key in keys)
            {
                await instance.SetAsync<T>(key, content, period, token);
            }
        }

        /// <summary>
        /// Simple remove multiple operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="keys">keys to be removed</param>
        public static void RemoveMultiKey(this ICarbonCache instance, IList<string> keys)
        {
            foreach (var key in keys)
            {
                instance.Remove(key);
            }
        }

        /// <summary>
        /// Simple remove multiple async operation which is inherited from IDistributedCache interface, all the set logic is managed by Microsoft.Extensions.Caching.Distributed.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="keys">keys to be removed</param>
        public static async Task RemoveMultiKeyAsync(this ICarbonCache instance, IList<string> keys, CancellationToken token = default(CancellationToken))
        {
            foreach (var key in keys)
            {
                await instance.RemoveAsync(key, token);
            }
        }

        #endregion

        private static string ToRedisInstanceKey(this string key, ICarbonCache instance)
        {
            return instance.GetInstanceName() + key;
        }

        private static string[] ToRedisInstanceKey(this string[] keys, ICarbonCache instance)
        {
            return keys.Select(k => k.ToRedisInstanceKey(instance)).ToArray();
        }
    }
}
