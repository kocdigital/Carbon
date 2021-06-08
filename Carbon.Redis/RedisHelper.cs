using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.Redis
{
    public static class RedisHelper
    {
        public static int RedisKeyLength = RedisConstants.KeyLength;

        public static void SetRedisKeyLength(int redisKeyLength)
        {
            RedisKeyLength = redisKeyLength;
        }
        /// <summary>
        /// Converts object to json.
        /// </summary>
        /// <returns> json string</returns>
        public static string ConvertObjectToJson(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// Gets data from Redis and converts it into the proper object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        public static async Task<T> ConvertJsonToObjectAsync<T>(this IDatabase database, string key)
        {
            try
            {
                var value = await database.StringGetAsync(key);
                if (!value.IsNull)
                    return JsonConvert.DeserializeObject<T>(value);
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {

                throw new RedisException("The object couldn't deserialized", ex);
            }

        }

        /// <summary>
        /// Gets data from Redis and converts it into the proper object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> ConvertJsonToObjectAsync<T>(this IDatabase database, IEnumerable<RedisKey> keys)
        {
            try
            {
                var values = await database.StringGetAsync(keys.ToArray());
                var resultList = new List<T>();

                if (values != null)
                {
                    resultList.AddRange(values?.Select(value => JsonConvert.DeserializeObject<T>(value)));
                }

                return resultList;
            }
            catch (Exception ex)
            {

                throw new RedisException("The object couldn't deserialized", ex);
            }

        }

        /// <summary>
        /// Is given key valid? <br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>isValid (<i>Is given key valid?</i>)</term>
        /// </item>
        /// <br>
        /// <item>
        /// <term>errorMessage(<i>Error Messages</i>)</term>
        /// </item>
        /// </br>
        /// </list>
        /// </br>
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public static (bool isValid, string error) IsKeyValid(this string key, IDatabase _redisDb)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, RedisConstants.RedisDisabled);
            }
            var keyLength = GetKeyLength();

            if (key.Length > keyLength)
            {
                return (false, $"key length has to be smaller than {keyLength}");
            }
            if (!key.Contains(":"))
            {
                return (false, "key should contains ':' character between meaningful seperations. The sample key is object-type:id:field(user:100:password)");
            }
            return (true, null);
        }
        /// <summary>
        /// Getting defined key length  from the configuration settings
        /// </summary>
        private static int GetKeyLength()
        {
            var keyLength = RedisConstants.KeyLength;

            if (RedisKeyLength > 0)
            {
                keyLength = RedisKeyLength;
            }
            return keyLength;
        }
        /// <summary>
        /// Is redis disabled or not
        /// </summary>
        public static bool IsRedisDisabled(this IDatabase db)
        {
            if (db.Multiplexer == null)
            {
                return true;
            }
            return false;
        }
    }
}
