using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Carbon.Redis.UnitTests.StaticWrappers.RedisHelper
{
    public interface IRedisHelperWrapper 
    {
        /// <summary>
        /// Converts object to json.
        /// </summary>
        /// <returns> json string</returns>
        public string ConvertObjectToJson(object obj);

        /// <summary>
        /// Converts json to given object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        public Task<T> ConvertJsonToObjectAsync<T>(IDatabase database, string key);

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
        public Task<(bool isValid, string error)> IsKeyValid(string key, IDatabase _redisDb);

        /// <summary>
        /// Is redis disabled or not
        /// </summary>
        public bool IsRedisDisabled(IDatabase db);

    }
}
