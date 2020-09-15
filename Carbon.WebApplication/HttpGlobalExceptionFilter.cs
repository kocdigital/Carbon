using Carbon.Common;
using Carbon.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Carbon.WebApplication
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        private const int GeneralServerErrorCode = (int)ApiStatusCode.InternalServerError;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Called after an action has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context">The <see cref="ExceptionContext"/>.</param>
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("X-CorrelationId", out var correlationId);

            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var apiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.InternalServerError);

            if (context.Exception is CarbonException)
            {
                var exception = (CarbonException)context.Exception;

                if (!string.IsNullOrEmpty(context.Exception.Message))
                {
                    apiResponse.AddMessage(context.Exception.Message);
                }

                apiResponse.SetErrorCode(exception.ErrorCode);
            }
            else
            {
                if (!string.IsNullOrEmpty(context.Exception.Message))
                {
                    apiResponse.AddMessage(context.Exception.Message);
                }

                apiResponse.SetErrorCode(GeneralServerErrorCode);
            }

            //if (_env.IsDevelopment())
            //{
            //    apiResponse.Messages.Add(context.Exception.Demystify().ToString());
            //}

            context.Result = new InternalServerErrorObjectResult(apiResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";

            context.ExceptionHandled = true;
        }
    }
}
