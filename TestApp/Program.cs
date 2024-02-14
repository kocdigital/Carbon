using Carbon.WebApplication;
using Carbon.WebApplication.Middlewares;
using Carbon.Caching.Abstractions;
using Carbon.Caching.Redis;
using Carbon.Redis;
using Mapster;
using TestApp.Application.Cache;
using TestApp.Application.Services;

namespace TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = @"CARBON_TEST_API";
            var builder = WebApplication.CreateBuilder(args);
            builder.AddCarbonServices((services) =>
            {
                services.AddRedisPersister(builder.Configuration).AddCarbonRedisCachingHelper(true);
                ICarbonCacheExtensions.SetSerializationType(CarbonContentSerializationType.Json);
                services.AddControllers();
                services.AddScoped<ICacheExpirationEventHandler, ExpirationHandler>();
                services.AddScoped<IRedisTestService, RedisTestService>();
                services.AddHostedService<CacheEventListener>();
                return services;
            });
            var app = builder.Build();
            app.AddCarbonApplication((application) =>
            {
                application.UseHsts();
                application.UseHttpsRedirection();

                return application;
            });

            app.Run();
        }
    }
}
