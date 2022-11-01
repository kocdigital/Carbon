using Carbon.Caching.Abstractions.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Caching.Abstractions
{
    public static class ICarbonCacheExtensions
    {
        private static Serialization SerializationType { get; set; }

        public static void SetSerializationType(Serialization PreferredSerializationType)
        {
            if (PreferredSerializationType == Serialization.BinaryFormatter)
            {
                Console.WriteLine(SerializationType + " is obsolete, handle with care or consider changing it!");
                SerializationType = PreferredSerializationType;
            }
            else if (PreferredSerializationType == Serialization.Json)
            {
                SerializationType = PreferredSerializationType;
            }
            else if (PreferredSerializationType == Serialization.Protobuf)
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
