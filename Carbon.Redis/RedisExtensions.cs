using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.Redis
{
    public static class RedisExtension
    {
        /// <summary>
        /// Delete multiple key. <br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>removedKeys (<i>key list that removed from cache</i>)</term>
        /// </item>
        /// <br>
        /// <item>
        /// <term>couldNotBeRemovedKeys(<i>Key list that cannot be removed from cache</i>)</term>
        /// </item>
        /// </br>
        /// <br>
        /// <item>
        /// <term>errorMessage(<i>Error Messages</i>)</term>
        /// </item>
        /// </br>
        /// </list> 
        /// </br>
        /// </summary>
        /// <param name="keyPattern">It represents cache key pattern. Sample pattern => Customer:b0cbb0a2-4feb-4655-aeb5-ce55fad71699:Home:*</param>
        /// <param name="_connectionMultiplexer">connectionMultiplexer object</param>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public static async Task<(List<string> removedKeys, List<string> couldNotBeRemovedKeys, string errorMessage)> RemoveKeysByPattern(this IDatabase _redisDb, string keyPattern, IConnectionMultiplexer _connectionMultiplexer)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, default, RedisConstants.RedisDisabled);
            }
            try
            {
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

                var keys = server.KeysAsync(pattern: keyPattern);

                var removedKeys = new List<string>();
                var couldNotBeRemovedKeys = new List<string>();
                await foreach (var key in keys)
                {
                    if (_redisDb.KeyDelete(key))
                    {
                        removedKeys.Add(key);
                    }
                    else
                    {
                        couldNotBeRemovedKeys.Add(key);
                    }
                }
                return (removedKeys, couldNotBeRemovedKeys, null);
            }
            catch (Exception ex)
            {
                return (default, default, ex.InnerException?.Message ?? ex.Message);
            }

        }
        /// <summary>
        /// Get cached object <br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>cachedObject (<i>cached object</i>)</term>
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
        public static async Task<(T cachedObject, string errorMessage)> Get<T>(this IDatabase _redisDb, string key)
        {
            var (isValid, error) = await key.IsKeyValid(_redisDb);

            if (!isValid)
            {
                return (default, error);
            }
            try
            {
                var result = await _redisDb.ConvertJsonToObjectAsync<T>(key);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (default, ex.InnerException?.Message ?? ex.Message);
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
        public static async Task<(bool isValid, string errorMessage)> IsCacheKeyValid(this IDatabase _redisDb, string key)
        {
            return await key.IsKeyValid(_redisDb);
        }
        /// <summary>
        /// Insert an object to the cache
        /// The key <strong>should be less than 1024 byte</strong> 
        /// and 
        /// key should contains <strong>':'</strong> character between meaningful seperations. 
        /// The sample key is <strong>object-type:id:field(user:100:password)</strong><br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>isSucess (<i> Is given object cached?</i>)</term>
        /// </item>
        /// <br>
        /// <item>
        /// <term>errorMessage(<i>Error Messages</i>)</term>
        /// </item>
        /// </br>
        /// </list> 
        /// </br>
        /// </summary>
        /// <param name="key">It represents cache key</param>
        /// <param name="data">It represents cache object value</param>
        /// <returns>  </returns>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public async static Task<(bool isSucess, string errorMessage)> Set<T>(this IDatabase _redisDb, string key, T data, TimeSpan? expiry = null)
        {
            var (isValid, error) = await key.IsKeyValid(_redisDb);

            if (!isValid)
            {
                return (false, error);
            }
            bool isSuccess;
            try
            {
                isSuccess = await _redisDb.StringSetAsync(key, data.ConvertObjectToJson(), expiry);
            }
            catch (Exception ex)
            {
                return (false, "The object couldn't cached - " + (ex.InnerException?.Message ?? ex.Message));
            }

            return (isSuccess, null);
        }
        /// <summary>
        /// Remove key and key's value from the cache
        /// The key <strong>should be less than 1024 byte</strong> 
        /// and 
        /// key should contains <strong>':'</strong> character between meaningful seperations. 
        /// The sample key is <strong>object-type:id:field(user:100:password)</strong><br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>isSucess (<i> Is given key removed from the cache?</i>)</term>
        /// </item>
        /// <br>
        /// <item>
        /// <term>errorMessage(<i>Error Messages</i>)</term>
        /// </item>
        /// </br>
        /// </list> 
        /// </br>
        /// </summary>
        /// <param name="key">It represents cache key</param>
        /// <returns>  </returns>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public async static Task<(bool isSuccess, string errorMessage)> RemoveKey(this IDatabase _redisDb, string key)
        {
            var (isValid, error) = await key.IsKeyValid(_redisDb);
            if (!isValid)
            {
                return (false, error);
            }
            try
            {
                var result = await _redisDb.KeyDeleteAsync(key);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (default, ex.InnerException?.Message ?? ex.Message);
            }
        }


    }

}
