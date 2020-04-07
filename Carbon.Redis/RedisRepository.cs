using Carbon.Redis.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.Redis
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDb;
        private readonly RedisSettings _redisSettings;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisRepository(IDatabase redisDb, IOptions<RedisSettings> redisSettings, IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDb = redisDb;
            _redisSettings = redisSettings.Value;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public  (List<string> removedKeys , List<string> couldNotBeRemovedKeys) RemoveKeysByPattern(string keyPattern)
        {
            var server =   _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
           
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
        public (T, string) GetComplexObject<T>(string key)
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return (default, error);
            }
            return (_redisDb.ConvertJsonToObject<T>(key), null);
        }

        public async Task<(bool, string)> SetComplexObject<T>(string key, T data)
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.StringSetAsync(key, data.ConvertObjectToJson()), null);
        }

        public async Task<(string, string)> StringGetAsync(string key)
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return (null, error);
            }
            return (await _redisDb.StringGetAsync(key), null);
        }

        public async Task<(bool, string)> StringSetAsync(string key, string value)
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.StringSetAsync(key, value), null);
        }
        public async Task<string> SetSimpleObjectAsync<T>(string key, T value) where T : new()
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return error;
            }
            var propList = value.ConvertToHashEntryList();
            var lst = propList.ToArray();
            await _redisDb.HashSetAsync(key, lst);
            return null;
        }
        public (T, string) GetSimpleObject<T>(string key) where T : new()
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
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
        public async Task<(bool, string)> RemoveKey(string key)
        {
            var (isValid, error) = key.IsKeyValid(_redisSettings.KeyLength);
            if (!isValid)
            {
                return (false, error);
            }
            return (await _redisDb.KeyDeleteAsync(key), null);
        }
    }

}
