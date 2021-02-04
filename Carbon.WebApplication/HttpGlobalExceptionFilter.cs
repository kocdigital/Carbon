using Carbon.Common;
using Carbon.ExceptionHandling.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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

            //_logger.LogError(new EventId(context.Exception.HResult),
            //    context.Exception,
            //    context.Exception.Message);

            

            var apiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.InternalServerError);

            if (context.Exception is CarbonException)
            {
                var exception = (CarbonException)context.Exception;

                _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{ "Args"}}} {{{ "StackTrace"}}}",
                    exception.ErrorCode, exception.Message, exception.SerializedModel, context.Exception.StackTrace);

                if (!string.IsNullOrEmpty(context.Exception.Message))
                {
                    apiResponse.AddMessage(context.Exception.Message);
                }

                if (_env.IsDevelopment() && !string.IsNullOrEmpty(context.Exception.StackTrace))
                {
                    apiResponse.AddMessage(context.Exception.StackTrace);
                }

                apiResponse.SetErrorCode(exception.ErrorCode);
            }
            else
            {
                _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{ "StackTrace"}}}",
                    GeneralServerErrorCode, context.Exception.Message, context.Exception.StackTrace);

                if (!string.IsNullOrEmpty(context.Exception.Message))
                {
                    apiResponse.AddMessage(context.Exception.Message);
                }

                if (!string.IsNullOrEmpty(context.Exception.StackTrace))
                {
                    apiResponse.AddMessage(context.Exception.StackTrace);
                }

                apiResponse.SetErrorCode(GeneralServerErrorCode);
            }

            if (context.Exception is ForbiddenOperationException)
            {
                var objectResult = new ObjectResult(apiResponse);
                objectResult.StatusCode = StatusCodes.Status403Forbidden;
                context.Result = objectResult;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.HttpContext.Response.ContentType = "application/json";
                context.ExceptionHandled = true;
            }
            else
            {
                context.Result = new InternalServerErrorObjectResult(apiResponse);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.HttpContext.Response.ContentType = "application/json";
                context.ExceptionHandled = true;
            }
        }
    }
}
