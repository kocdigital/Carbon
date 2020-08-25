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
        /// If you want to delete multiple key, use this method. <br>Returns:</br>
        /// <br><list type="bullet"><item><term>removedKeys</term></item><br><item><term>couldNotBeRemovedKeys</term></item></br></list> </br>
        /// </summary>
        /// <param name="keyPattern">It represents cache key</param>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public static async Task<(List<string> removedKeys, List<string> couldNotBeRemovedKeys, string errorMessage)> RemoveKeysByPattern(this IDatabase _redisDb, string keyPattern, IConnectionMultiplexer _connectionMultiplexer)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, default, "Redis is disabled");
            }
            try
            {
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

                var keys = server.KeysAsync(pattern: keyPattern);

                var removedKeys = new List<string>();
                var couldNotBeRemovedKeys = new List<string>();
                //todo: batch delete
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
                return (default, default, ex.InnerException?.Message??ex.Message);
            }
            
        }
        public static async Task<(T, string)> Get<T>(this IDatabase _redisDb, string key)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, "Redis is disabled");
            }
            var (isValid, error) = key.IsKeyValid(_redisDb);

            if (!isValid)
            {
                return (default, error);
            }
            return (await _redisDb.ConvertJsonToObjectAsync<T>(key), null);
        }
        /// <summary>
        /// If you want to insert a complex object  to cache, use this method. For this method the object should contains another child object or list of object.
        /// The key <strong>should be less than 1024 byte</strong> 
        /// and 
        /// key should contains <strong>':'</strong> character between meaningful seperations. 
        /// The sample key is <strong>object-type:id:field(user:100:password)</strong>
        /// </summary>
        /// <param name="key">It represents cache key</param>
        /// <param name="data">It represents cache object value</param>
        /// <returns>  </returns>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public async static Task<(bool isSucess, string errorMessage)> Set<T>(this IDatabase _redisDb, string key, T data, TimeSpan? expiry = null)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, "Redis is disabled");
            }
            var (isValid, error) = key.IsKeyValid(_redisDb);

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
                throw new RedisException("The object couldn't cached" ,ex);
            }
          
            return (isSuccess, null);
        }
     
        /// <summary>
        /// If you want to insert a simple object  to cache, use this method. For this method the object shouldn't contains another child object or list of object.
        /// The key <strong>should be less than 1024 byte</strong> 
        /// and 
        /// key should contains <strong>':'</strong> character between meaningful seperations. 
        /// The sample key is <strong>object-type:id:field(user:100:password)</strong>
        /// <br>Another sample is <strong>"Fault:{0}:FaultDetail:{1}"</strong> if we want to delete FaultDetails of a Fault, it becomes easier with this pattern.</br>
        /// <br>If there is no error is returns null</br>
        /// </summary>
        /// <param name="key">It represents cache key</param>
        /// <param name="value">It represents cache object value</param>
        /// <returns>  </returns>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public async static Task<string> SetHashAsync<T>(this IDatabase _redisDb, string key, T value, DateTime? expiry = null) where T : new()
        {
            if(_redisDb.IsRedisDisabled())
            {
                return "Redis is disabled";
            }

            var (isValid, error) = key.IsKeyValid(_redisDb);

            if (!isValid)
            {
                return error;
            }
            var propList = value.ConvertToHashEntryList();
            var lst = propList.ToArray();

             await _redisDb.HashSetAsync(key, lst);
            if (expiry.HasValue)
            {
                var isExpireSet = await _redisDb.KeyExpireAsync(key, expiry);
                if (!isExpireSet)
                {
                    return $"Expire date ( {expiry} ) couldn't set";
                }
            }
            return null;
        }
        public static async Task<(T data, string errorMessage)> GetHash<T>(this IDatabase _redisDb, string key) where T : new()
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default,"Redis is disabled");
            }
            var (isValid, error) = key.IsKeyValid(_redisDb);

            if (!isValid)
            {
                return (default, error);
            }
            var hash = await _redisDb.HashGetAllAsync(key);
            if (hash!= null && hash.Any())
            {
                return (hash.ConvertFromHashEntryList<T>(), null);
            }
            return (default, null);
        }
        public async static Task<(bool isSuccess, string errorMessage)> RemoveKey(this IDatabase _redisDb, string key)
        {
            if (_redisDb.IsRedisDisabled())
            {
                return (default, "Redis is disabled");
            }
            var (isValid, error) = key.IsKeyValid(_redisDb);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.KeyDeleteAsync(key), null);
        }

        
    }

}
