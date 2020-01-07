using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{

    public class BadRequestValidationResult : IActionResult
    {
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
