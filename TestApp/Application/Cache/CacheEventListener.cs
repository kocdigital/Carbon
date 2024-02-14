using Carbon.Caching.Redis;

namespace TestApp.Application.Cache
{
    public interface ICacheExpirationEventHandler
    {
        string HandlerKey { get; }
        Task HandleExpirationEvent(string eventKey);
    }
    public class CacheEventListener : IHostedService
    {
        private readonly ILogger<CacheEventListener> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICarbonRedisCache _cache;

        public CacheEventListener(ILogger<CacheEventListener> logger, IServiceProvider serviceProvider, ICarbonRedisCache cache)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cache.RegisterOnExpiredEventHandler(OnExpired);

            _logger.LogInformation("Starting CacheEventListener");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping CacheEventListener");
            return Task.CompletedTask;
        }

        private CarbonRedisCache.OnExpiredHandler OnExpired => async eventKey =>
        {
            _logger.LogInformation("CacheEventListener: Cache Key {EventKey} expired", eventKey);

            var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider.GetServices<ICacheExpirationEventHandler>();
            var cacheEventHandler = services.FirstOrDefault(x => eventKey.Contains(x.HandlerKey));
            if (cacheEventHandler != null)
            {
                _logger.LogInformation("CacheEventListener: {EventKey} triggered", eventKey);
                await cacheEventHandler.HandleExpirationEvent(eventKey);
            }
        };
    }
}
