using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Carbon.Redis
{
    public static class RedisExtension
    {
        /// <summary>
        /// Get multiple data by pattern  <br>Returns:</br>
        /// <br>
        /// <list type="bullet">
        /// <item>
        /// <term>dataList(<i>List of return data</i>)</term>
        /// </item>
        /// <br>
        /// <item>
        /// <term>errorMessage(<i>Error Messages</i>)</term>
        /// </item>
        /// </br>
        /// </list> 
        /// </br>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisDb">database object</param>
        /// <param name="keyPattern">It represents cache key pattern. Sample pattern => Customer:b0cbb0a2-4feb-4655-aeb5-ce55fad71699:Home:*</param>
        /// <param name="connectionMultiplexer">connectionMultiplexer object</param>
        /// <returns></returns>
        public static async Task<(IEnumerable<T> dataList, string errorMessage)> GetByPattern<T>(this IDatabase redisDb, string keyPattern, IConnectionMultiplexer connectionMultiplexer)
        {
            if (redisDb.IsRedisDisabled())
            {
                return (default, RedisConstants.RedisDisabled);
            }
            try
            {
                var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());

                var keys = server.KeysAsync(database: connectionMultiplexer.GetDatabase().Database, pattern: keyPattern) as IEnumerable<RedisKey>;

                var dataList = await redisDb.ConvertJsonToObjectAsync<T>(keys);

                return (dataList.ToList(), null);
            }
            catch (Exception ex)
            {
                return (default, ex.InnerException?.Message ?? ex.Message);
            }

        }

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
        /// <param name="redisDb">database object</param>
        /// <param name="keyPattern">It represents cache key pattern. Sample pattern => Customer:b0cbb0a2-4feb-4655-aeb5-ce55fad71699:Home:*</param>
        /// <param name="connectionMultiplexer">connectionMultiplexer object</param>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public static async Task<(List<string> removedKeys, List<string> couldNotBeRemovedKeys, string errorMessage)> RemoveKeysByPattern(this IDatabase redisDb, string keyPattern, IConnectionMultiplexer connectionMultiplexer)
        {
            if (redisDb.IsRedisDisabled())
            {
                return (default, default, RedisConstants.RedisDisabled);
            }
            try
            {
                var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());

                var keys = server.KeysAsync(database: connectionMultiplexer.GetDatabase().Database, pattern: keyPattern);

                var removedKeys = new List<string>();
                var couldNotBeRemovedKeys = new List<string>();
                await foreach (var key in keys)
                {
                    if (redisDb.KeyDelete(key))
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
        public static async Task<(T cachedObject, string errorMessage)> Get<T>(this IDatabase redisDb, string key)
        {
            var (isValid, error) = await key.IsKeyValid(redisDb);

            if (!isValid)
            {
                return (default, error);
            }
            try
            {
                var result = await redisDb.ConvertJsonToObjectAsync<T>(key);
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
        public static async Task<(bool isValid, string errorMessage)> IsCacheKeyValid(this IDatabase redisDb, string key)
        {
            return await key.IsKeyValid(redisDb);
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
        public async static Task<(bool isSucess, string errorMessage)> Set<T>(this IDatabase redisDb, string key, T data, TimeSpan? expiry = null)
        {
            var (isValid, error) = await key.IsKeyValid(redisDb);

            if (!isValid)
            {
                return (false, error);
            }
            bool isSuccess;
            try
            {
                isSuccess = await redisDb.StringSetAsync(key, data.ConvertObjectToJson(), expiry);
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
        /// key should contains <strong>':'</strong> character between meaningful separations. 
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
        public async static Task<(bool isSuccess, string errorMessage)> RemoveKey(this IDatabase redisDb, string key)
        {
            var (isValid, error) = await key.IsKeyValid(redisDb);
            if (!isValid)
            {
                return (false, error);
            }
            try
            {
                var result = await redisDb.KeyDeleteAsync(key);
                return (result, null);
            }
            catch (Exception ex)
            {
                return (default, ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Get cache with logger.
        /// <br/>
        /// <example>After class initialization, call this method:
        /// <code>
        /// var cacheObject = new CacheObject&lt;T&gt;(cacheKeyPattern, dto.TenantId, _cache, _logger);
        /// <br/>
        /// var (data, message) = await cacheObject.GetWithLogging&lt;T&gt;<typeparamref name="T"/>();
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">the type of expected data object</typeparam>
        /// <param name="cacheObject"></param>
        /// <returns>a boolean isSuccess flag and an error message.</returns>
        public static async Task<(T cachedData, string errorMessage)> GetWithLogging<T>(this CacheObject cacheObject)
        {
            if (cacheObject == null || string.IsNullOrWhiteSpace(cacheObject.CacheKey))
            {
                cacheObject?.Logger?.LogError($"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    ErrorMessageConstants.CacheRequestIsNotValid);

                return (default, ErrorMessageConstants.CacheRequestIsNotValid);
            }

            var (cachedData, errorMessage) = await cacheObject.Cache.Get<T>(cacheObject.CacheKey);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                cacheObject?.Logger?.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
                return (default, $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
            }

            return (cachedData, null);
        }

        /// <summary>
        /// Get cache with logger.
        /// <br/>
        /// <example>After class initialization, call this method:
        /// <code>
        /// var cacheObject = new CacheObject&lt;T&gt;(cacheKeyPattern, dto.TenantId, _cache, _logger);
        /// <br/>
        /// var (data, message) = await cacheObject.GetWithLogging&lt;T&gt;<typeparamref name="T"/>();
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">the type of expected data object</typeparam>
        /// <param name="cacheObject"></param>
        /// <param name="connectionMultiplexer"></param>
        /// <returns>a boolean isSuccess flag and an error message.</returns>
        public static async Task<(IEnumerable<T> cachedDataList, string errorMessage)> GetByPatternWithLogging<T>(this CacheObject cacheObject, IConnectionMultiplexer connectionMultiplexer)
        {
            if (cacheObject == null || string.IsNullOrWhiteSpace(cacheObject.CacheKey))
            {
                cacheObject?.Logger?.LogError($"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    ErrorMessageConstants.CacheRequestIsNotValid);

                return (default, ErrorMessageConstants.CacheRequestIsNotValid);
            }

            var (cachedDataList, errorMessage) = await cacheObject.Cache.GetByPattern<T>(cacheObject.CacheKey, connectionMultiplexer);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                cacheObject?.Logger?.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
                return (default, $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
            }

            return (cachedDataList, null);
        }

        /// <summary>
        /// Set cache with logger.
        /// <br/>
        /// <example>After class initialization, call this method:
        /// <code>
        /// var cacheObject = new CacheObject&lt;T&gt;(cacheKey, dto.TenantId, _cache, _logger);
        /// <br/>
        /// var (isSuccess, message) = await cacheObject.SetWithLogging&lt;T&gt;(data);
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T">the type of object to be cached</typeparam>
        /// <param name="cacheObject"></param>
        /// <param name="data">the object to be cached</param>
        /// <returns>a boolean isSuccess flag and an error message.</returns>
        public static async Task<(bool isSuccess, string errorMessage)> SetWithLogging<T>(this CacheObject cacheObject, T data)
        {
            if (cacheObject == null || string.IsNullOrWhiteSpace(cacheObject.CacheKey) || data == null)
            {
                cacheObject?.Logger?.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}} {{{SerilogConstant.RequestBody}}} {{{SerilogConstant.Error}}}",
                    cacheObject?.CacheKey, cacheObject.TenantId, JsonConvert.SerializeObject(cacheObject),
                    ErrorMessageConstants.CacheRequestIsNotValid);
                return (false, ErrorMessageConstants.CacheRequestIsNotValid);
            }

            var (isSuccess, setError) =
                await cacheObject.Cache.Set(cacheObject.CacheKey, data);

            if (!isSuccess)
            {
                cacheObject.Logger?.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}} {{{SerilogConstant.RequestBody}}} {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId, JsonConvert.SerializeObject(cacheObject), setError);
                return (false, setError);
            }

            return (true, null);
        }

        /// <summary>
        /// Delete multiple cache data by pattern with logger.
        /// <br/>
        /// <example>After class initialization, call this method:
        /// <code>
        /// var cacheObject = new CacheObject&lt;T&gt;(cacheKeyPattern, dto.TenantId, _cache, _logger);
        /// <br/>
        /// var (isSuccess, message) = await cacheObject.RemoveKeysByPatternWithLogging(connectionMultiplexer);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <param name="connectionMultiplexer">the connection multiplexer of the proper Redis Server</param>
        /// <returns>a boolean isSuccess flag and an error message.</returns>
        public static async Task<(bool isSuccess, string errorMessage)> RemoveKeysByPatternWithLogging(this CacheObject cacheObject, IConnectionMultiplexer connectionMultiplexer)
        {
            if (cacheObject == null || string.IsNullOrWhiteSpace(cacheObject.CacheKey) || connectionMultiplexer == null)
            {
                cacheObject?.Logger?.LogError($"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    ErrorMessageConstants.CacheRequestIsNotValid);

                return (false, ErrorMessageConstants.CacheRequestIsNotValid);
            }

            var (_, couldNotBeRemoved, errorMessage) = await cacheObject.Cache.RemoveKeysByPattern(cacheObject.CacheKey, connectionMultiplexer);

            if (couldNotBeRemoved != null && couldNotBeRemoved.Any())
            {
                var serializedObject = JsonConvert.SerializeObject(couldNotBeRemoved);
                cacheObject.Logger.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    $"Error Message: {errorMessage} - Object: {serializedObject}");
                return (false, $"Error Message: {errorMessage} - Object: {serializedObject}");
            }

            return (true, null);
        }

        /// <summary>
        /// Delete cache with logger.
        /// <br/>
        /// <example>After class initialization, call this method:
        /// <code>
        /// var cacheObject = new CacheObject&lt;T&gt;(cacheKeyPattern, dto.TenantId, _cache, _logger);
        /// <br/>
        /// var (isSuccess, message) = await cacheObject.RemoveKeyWithLogging();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="cacheObject"></param>
        /// <returns>a boolean isSuccess flag and an error message.</returns>
        public static async Task<(bool isSuccess, string errorMessage)> RemoveKeyWithLogging(this CacheObject cacheObject)
        {
            if (cacheObject == null || string.IsNullOrWhiteSpace(cacheObject.CacheKey))
            {
                cacheObject?.Logger?.LogError($"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    ErrorMessageConstants.CacheRequestIsNotValid);

                return (false, ErrorMessageConstants.CacheRequestIsNotValid);
            }

            var (isSuccess, errorMessage) = await cacheObject.Cache.RemoveKey(cacheObject.CacheKey);

            if (!isSuccess)
            {
                cacheObject?.Logger?.LogError(
                    $"{{{SerilogConstant.CacheKey}}} {{{SerilogConstant.TenantId}}}  {{{SerilogConstant.Error}}}",
                    cacheObject.CacheKey, cacheObject.TenantId,
                    $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
                return (false, $"Error Message: {errorMessage} - Cache Key: {cacheObject.CacheKey}");
            }

            return (true, null);
        }


    }

}
