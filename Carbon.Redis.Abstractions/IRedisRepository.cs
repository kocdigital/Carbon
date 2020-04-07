using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.Redis.Abstractions
{
    public interface IRedisRepository
    {
        /// <summary>
        /// If you want to delete multiple key, use this method. <br>Returns:</br>
        /// <br><list type="bullet"><item><term>removedKeys</term></item><br><item><term>couldNotBeRemovedKeys</term></item></br></list> </br>
        /// </summary>
        /// <param name="keyPattern">It represents cache key</param>
        (List<string> removedKeys , List<string> couldNotBeRemovedKeys) RemoveKeysByPattern(string keyPattern);
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
        Task<(bool isSuccess, string error)> SetComplexObject<T>(string key, T data);
        (T data, string error) GetComplexObject<T>(string key);
        /// <summary>
        /// If you want to insert string value to cache, use this method. <br>The key <strong>should be less than 1024 byte</strong> 
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
        Task<(bool isSuccess, string error)> StringSetAsync(string key, string value);
        Task<(string data, string error)> StringGetAsync(string key);
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
        Task<string> SetSimpleObjectAsync<T>(string key, T value) where T : new();
        (T data, string error) GetSimpleObject<T>(string key) where T : new();
        Task<(bool isDeleted, string error)> RemoveKey(string key);
    }
}
