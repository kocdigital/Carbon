using Carbon.Redis;
using Carbon.WebApplication;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carbon.Demo.WebApplication.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : CarbonController
    {
        private readonly IDatabase _redis;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisController(IDatabase redis, IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = redis;
            _connectionMultiplexer = connectionMultiplexer;
        }


        [HttpGet]
        [Route("Cache")]
        public async Task<string> Cache()
        {
            var home = new Home()
            {
                Address = "Koç Digital"
            };
            var homeForSummer = new Home()
            {
                Address = "Hawaii"
            };

            var customer = new Customer()
            {
                Age = 21,
                Name = "Bilgehan",
                Home = new List<Home>() { home, homeForSummer }
            };
            var sasas = _redis.StringSet("aaa", 122);
            var sassas = _redis.StringGet("aaa");
       

            var (setComplexIsSuccess, setComplexError) = await _redis.Set(string.Format(CacheKey.CustomerById, customer.Id), customer);

           
            var (customerData, customerError) = await _redis.Get<Customer>(string.Format(CacheKey.CustomerById, customer.Id));

            var customer2 = new Customer()
            {
                Age = 21,
                Name = "Bilgehan",
                Home = new List<Home>() { home, homeForSummer }
            };
            var (setComplexIsSuccess1, setComplexError1) = await _redis.Set(string.Format(CacheKey.CustomerById, customer2.Id), customer);
            var (isDeleted, errorRemove) = await _redis.RemoveKey(string.Format(CacheKey.CustomerById, customer2.Id));
            var (ss, dd) = await _redis.IsCacheKeyValid("ddfsfsf");
            var (removedList, couldNotBeRemoved, errorMessage) = await _redis.RemoveKeysByPattern(string.Format(CacheKey.CustomerHome, customer.Id, "*"), _connectionMultiplexer);
             (removedList, couldNotBeRemoved, errorMessage) = await _redis.ScanKeysAndRemoveByPattern(string.Format(CacheKey.CustomerHome, customer.Id, "*"));
            return null;
        }

        public class Customer
        {
            public Customer()
            {
                Id = Guid.NewGuid();
            }
            public Guid Id { get; set; }
            public int Age { get; set; }
            public string Name { get; set; }
            public List<Home> Home { get; set; }
        }
        public class Home
        {
            public Home()
            {
                Id = Guid.NewGuid();
            }
            public Guid Id { get; set; }
            public string Address { get; set; }
        }

        public static class CacheKey
        {
            public const string HomeAddressById = "Home:{0}:Address";

            public const string CustomerById = "Customer:{0}";
            public const string CustomerHome = "Customer:{0}:Home:{1}";
        }
    }
}