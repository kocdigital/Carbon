using Carbon.Caching.Abstractions;
using static Carbon.Caching.Redis.CarbonRedisCache;

namespace Carbon.Caching.Redis
{
	public interface ICarbonRedisCache : ICarbonCache
	{
		bool RegisterOnExpiredEventHandler(OnExpiredHandler handler);

	}
}
