using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Carbon.Redis
{
    /// <summary>
    /// The object for getting, setting or deleting cache data with logging purpose. 
    /// </summary>
    public class CacheObject
    {
        /// <summary>
        /// The constructor of CacheObject with compulsory parameters.
        /// </summary>
        /// <param name="cacheKey">The cache key or key pattern for getting, setting or deleting cache</param>
        /// <param name="tenantId">Tenant Id or Customer Id for logging purpose. It can also be set to Guid.Empty.</param>
        /// <param name="cache">An interface used to perform caching. <see cref="StackExchange.Redis.IDatabase"/></param>
        /// <param name="logger">An interface used to perform logging. <see cref="Microsoft.Extensions.Logging.ILogger"/></param>
        public CacheObject(string cacheKey, Guid tenantId, IDatabase cache, ILogger logger)
        {
            CacheKey = cacheKey;
            TenantId = tenantId;
            Cache = cache;
            Logger = logger;
        }

        /// <summary>
        /// An interface used to perform logging. <see cref="Microsoft.Extensions.Logging.ILogger"/>
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// An interface used to perform caching. <see cref="StackExchange.Redis.IDatabase"/>
        /// </summary>
        public IDatabase Cache { get; }

        /// <summary>
        /// Tenant Id or Customer Id for logging purpose. It can also be Guid.Empty.
        /// </summary>
        public Guid TenantId { get; }

        /// <summary>
        /// The cache key or key pattern for getting, setting or deleting cache
        /// </summary>
        public string CacheKey { get; }
    }
}
