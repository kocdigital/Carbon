using Carbon.Common;
using Carbon.ExceptionHandling.Abstractions;
using Carbon.WebApplication.TenantManagementHandler.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Carbon.WebApplication.TenantManagementHandler.Extensions;

namespace Carbon.WebApplication
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly IExternalService _externalService;
        private readonly IConfiguration _config;
        private readonly LogLevel logLevel;

        private const int GeneralServerErrorCode = (int)ApiStatusCode.InternalServerError;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, IExternalService externalService, IConfiguration configuration)
        {
            _env = env;
            _logger = logger;
            _externalService = externalService;
            _config = configuration;
            logLevel = configuration.GetSection("Logging:LogLevel").GetValue<LogLevel>("Default");
        }


        private async Task<string> GetErrorMessage(int errorCode, HttpRequest context)
        {
            var errorResponse = await
                _externalService
                .GetErrorDescription(new TenantManagementHandler.Dtos.ErrorHandling.ApplicationErrorRequest
            (
                 _env.ApplicationName,
                 errorCode,
                 context.GetUserLanguage(),
                 context.Headers["p360-solution-id"],
                 context.Headers["tenantId"]
            ));


            if (errorResponse != null)
            {
                return errorResponse.ErrorDescription;
            }
            return null;
        }


        /// <summary>
        /// Called after an action has thrown an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="context">The <see cref="ExceptionContext"/>.</param>
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Request.Headers.TryGetValue("X-CorrelationId", out var correlationId);

            var apiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.InternalServerError);

            if (context.Exception is CarbonException exception)
            {
                var exceptionMessage = GetErrorMessage(exception.ErrorCode, context.HttpContext.Request).Result;

                _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{ "Args"}}} {{{ "StackTrace"}}}",
                    exception.ErrorCode, exceptionMessage, exception.SerializedModel, context.Exception.StackTrace);

                //if (!string.IsNullOrEmpty(context.Exception.Message))
                //{
                //    apiResponse.AddMessage(context.Exception.Message);
                //}

                if (!string.IsNullOrEmpty(exceptionMessage))
                {
                    apiResponse.AddMessage(exceptionMessage);
                }

                if (logLevel == LogLevel.Trace && !string.IsNullOrEmpty(context.Exception.StackTrace))
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

                if (logLevel == LogLevel.Trace && !string.IsNullOrEmpty(context.Exception.StackTrace))
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
