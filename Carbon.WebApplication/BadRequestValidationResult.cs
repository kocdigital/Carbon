using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{
    public class BadRequestValidationResult : IActionResult
    {
        private readonly ILogger<BadRequestValidationResult> _logger;
        public BadRequestValidationResult(ILogger<BadRequestValidationResult> logger)
        {
            _logger = logger;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Request Validation Error",
                Instance = context.HttpContext.Request.Path
            };

            var result = new BadRequestObjectResult(problemDetails);
            result.ContentTypes.Add(MediaTypeNames.Application.Json);

            await result.ExecuteResultAsync(context);
        }
    }
}
