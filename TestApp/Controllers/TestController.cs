using Carbon.WebApplication;
using Carbon.WebApplication.HttpAtrributes;

using Microsoft.AspNetCore.Mvc;

using TestApp.Application.Services;

namespace TestApp.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : CarbonController
    {
        private readonly IRedisTestService _redisTestService;
        public TestController(IRedisTestService redisTestService)
        {
            _redisTestService = redisTestService;
        }
        [HttpGetCarbon]
        [Route("WriteToRedis")]
        public async Task<IActionResult> WriteToRedis([FromQuery]short ttl = 5)
        {
            var resp = await _redisTestService.WriteToRedis(ttl);
            return ResponseOk(resp);
        }
    }
}