using Carbon.Common;
using Carbon.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Carbon.WebApplication
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private const int GeneralServerErrorCode = 5000;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("x-idetifier", out var requestIdentifier);

            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var apiResponse = new ApiResponse<object>();

            if (context.Exception.GetType() == typeof(CarbonException))
            {
                var exception = (CarbonException)context.Exception;

                apiResponse.Messages = new List<string> { context.Exception.Message };
                apiResponse.Identifier = requestIdentifier;
                apiResponse.ErrorCode = exception.ErrorCode;
            }
            else
            {
                apiResponse.Messages = new List<string> { context.Exception.Message };
                apiResponse.Identifier = requestIdentifier;
                apiResponse.ErrorCode = GeneralServerErrorCode;
            }

            if (_env.IsDevelopment())
            {
                apiResponse.Messages.Add(context.Exception.Demystify().ToString());
            }

            context.Result = new InternalServerErrorObjectResult(apiResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";

            context.ExceptionHandled = true;
        }
    }
}
