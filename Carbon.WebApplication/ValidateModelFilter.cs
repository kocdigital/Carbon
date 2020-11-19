using Carbon.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Carbon.WebApplication
{
    public class ValidateModelFilter : IActionFilter
    {
        private const int GeneralClientErrorCode = 4000;


        /// <summary>
        /// Called after the action executes, before the action result.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutedContext"/>.</param>
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.HttpContext.Request.Headers.TryGetValue("X-CorrelationId", out var correlationId);

                var messages = new ValidationProblemDetails(context.ModelState).Errors.SelectMany(x => x.Value).ToList();
                var apiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.BadRequest, GeneralClientErrorCode, messages);

                context.Result = new BadRequestObjectResult(apiResponse);
            }
        }
    }
}