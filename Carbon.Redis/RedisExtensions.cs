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
        public static (List<string> removedKeys, List<string> couldNotBeRemovedKeys) RemoveKeysByPattern(this IDatabase _redisDb, string keyPattern, IConnectionMultiplexer _connectionMultiplexer)
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

            var keys = server.Keys(pattern: keyPattern).ToList();

            var removedKeys = new List<string>();
            var couldNotBeRemovedKeys = new List<string>();
            //todo: batch delete
            foreach (var key in keys)
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
            return (removedKeys, couldNotBeRemovedKeys);
        }
        public static (T, string) GetComplexObject<T>(this IDatabase _redisDb, string key)
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (default, error);
            }
            return (_redisDb.ConvertJsonToObject<T>(key), null);
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
        public async static Task<(bool, string)> SetComplexObject<T>(this IDatabase _redisDb, string key, T data, TimeSpan? expiry = null)
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (false, error);
            }

            return (await _redisDb.StringSetAsync(key, data.ConvertObjectToJson(), expiry), null);
        }
      
        public async static Task<(string, string)> GetBasicValueAsync(this IDatabase _redisDb, string key)
        {

            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (null, error);
            }
            return (await _redisDb.StringGetAsync(key), null);
        }
        /// <summary>
        /// If you want to insert basic type (string, integer, boolean ext. ) value to cache, use this method. <br>The key <strong>should be less than 1024 byte</strong> 
        /// and 
        /// key should contains <strong>':'</strong> character between meaningful seperations. </br>
        /// <br>The sample key is <strong>object-type:id:field(user:100:password)</strong></br> 
        /// <br>Another sample is <strong>"Fault:{0}:FaultDetail:{1}"</strong> if we want to delete FaultDetails of a Fault, it becomes easier with this pattern.</br>
        /// <br><see cref="https://redis.io/topics/data-types-intro"/></br>
        /// </summary>
        /// <param name="key">It represents cache key</param>
        /// <param name="value">It represents cache string value</param>
        /// <returns>  </returns>
        /// <see cref="https://redis.io/topics/data-types-intro"/>
        public async static Task<(bool, string)> SetBasicValueAsync(this IDatabase _redisDb, string key, string value, TimeSpan? expiry = null)
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.StringSetAsync(key, value, expiry), null);
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
        public async static Task<string> SetSimpleObjectAsync<T>(this IDatabase _redisDb, string key, T value, DateTime? expiry = null) where T : new()
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
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
        public static (T, string) GetSimpleObject<T>(this IDatabase _redisDb, string key) where T : new()
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (default, error);
            }
            var hash = _redisDb.HashGetAll(key).ToList();
            if (hash.Any())
            {
                return (hash.ConvertFromHashEntryList<T>(), null);
            }
            return (default, null);
        }
        public async static Task<(bool, string)> RemoveKey(this IDatabase _redisDb, string key)
        {
            var keyLength = GetKeyLength(_redisDb);
            var (isValid, error) = key.IsKeyValid(keyLength);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.KeyDeleteAsync(key), null);
        }

        private static int GetKeyLength(this IDatabase db)
        {
            var keyLength = RedisSettingConstants.KeyLength;
            var value = db.StringGet("redisKeyLength");
            if (value.HasValue && !value.IsNullOrEmpty && value != 0)
            {
                keyLength = (int)value;
            }

            return keyLength;
        }
    }

}
