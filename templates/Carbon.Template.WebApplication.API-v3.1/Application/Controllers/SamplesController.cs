using Carbon.WebApplication;
using Carbon.WebApplication.HttpAtrributes;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Carbon.Template.WebApplication.API_v3._1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SamplesController : CarbonController
    {
        private readonly ILogger<SamplesController> _logger;

        public SamplesController(ILogger<SamplesController> logger)
        {
            _logger = logger;
        }

        [HttpGetCarbon]
        public async Task<IActionResult> GetTest()
        {
            _logger.LogInformation("Test method started!");

            var dto = new { CustomerId = 1, Name = "TestCustomer" };
            return ResponseOk(dto);
        }
    }
}
