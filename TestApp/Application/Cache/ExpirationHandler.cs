using Carbon.Caching.Abstractions;
using Carbon.Caching.Redis;

namespace TestApp.Application.Cache
{
    public class ExpirationHandler : ICacheExpirationEventHandler
    {
        public string HandlerKey => "ae_test";

        private readonly ICarbonCache _cache;

        public ExpirationHandler(ICarbonCache cache)
        {
            _cache = cache;
        }

        public async Task HandleExpirationEvent(string eventKey)
        {
            using (var redlock = await _cache.GetRedLockFactory().CreateLockAsync(eventKey,
                       TimeSpan.FromSeconds(30),
                       TimeSpan.FromSeconds(30),
                       TimeSpan.FromSeconds(1)))
            {
                try
                {
                    if (redlock.IsAcquired)
                    {
                        Console.WriteLine($"Redis event received! Key:{eventKey}");
                    }
                }
                catch (Exception e)
                {
                    redlock.Dispose();
                }
            }
        }
    }
}
