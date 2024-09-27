using Carbon.Common;
using Carbon.ExceptionHandling.Abstractions;
using Carbon.WebApplication.TenantManagementHandler.Extensions;
using Carbon.WebApplication.TenantManagementHandler.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{
    /// <summary>
	/// Caches unhandled exceptions and builds a proper exception. Uses error descriptions, if "ErrorHandling:Url" defined at configuration
	/// <para>
	/// <see cref="CarbonStartup{TStartup}"/> uses this by default, adds as a MVC Filter.
	/// </para>
	/// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly IExternalService _externalService;
        private readonly IConfiguration _config;
        private readonly LogLevel logLevel;
        private readonly string _errorHandling;


        private const int GeneralServerErrorCode = (int)ApiStatusCode.InternalServerError;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, IExternalService externalService, IConfiguration configuration)
        {
            _env = env;
            _logger = logger;
            _externalService = externalService;
            _config = configuration;
            logLevel = configuration.GetSection("Logging:LogLevel").GetValue<LogLevel>("Default");
            _errorHandling = _config.GetValue<string>("ErrorHandling:Url");

        }


        private async Task<string> GetErrorMessage(int errorCode, object[] arguments, HttpRequest context)
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
                if (arguments?.Length > 0)
                {
                    var errorDescription = string.Format(errorResponse.ErrorDescription, arguments);
                    return errorDescription;
                }
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
            var correlationId = context.HttpContext.Request.GetIdentifier();

            var apiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.InternalServerError);

            if (context.Exception is CarbonException exception)
            {
                if (!string.IsNullOrEmpty(_errorHandling) && exception.OverrideExceptionDetail)
                {
                    var exceptionMessage = GetErrorMessage(exception.ErrorCode, exception.Arguments, context.HttpContext.Request).Result;

                    _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{"CorrelationId"}}} {{{ "Args"}}} {{{ "StackTrace"}}}",
                    exception.ErrorCode, exceptionMessage,correlationId, exception.SerializedModel, context.Exception.StackTrace);

                    if (!string.IsNullOrEmpty(exceptionMessage))
                    {
                        apiResponse.AddMessage(exceptionMessage);
                    }
                }
                else
                {
                    _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{"CorrelationId"}}} {{{ "Args"}}} {{{ "StackTrace"}}}",
                  exception.ErrorCode, context.Exception.Message,correlationId, exception.SerializedModel, context.Exception.StackTrace);

                    if (!string.IsNullOrEmpty(context.Exception.Message))
                    {
                        apiResponse.AddMessage(context.Exception.Message);
                    }
                }

                if (logLevel == LogLevel.Trace && !string.IsNullOrEmpty(context.Exception.StackTrace))
                {
                    apiResponse.AddMessage(context.Exception.StackTrace);
                }

                apiResponse.SetErrorCode(exception.ErrorCode);
            }
            else
            {
                _logger.LogError($" {{{ "ErrorCode"}}} {{{ "ErrorMessage"}}} {{{"CorrelationId"}}} {{{ "StackTrace"}}}",
                    GeneralServerErrorCode, context.Exception.Message, correlationId, context.Exception.StackTrace);

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
            else if (context.Exception is UnauthorizedOperationException)
            {
                var unAuthorizedApiResponse = new ApiResponse<object>(correlationId, ApiStatusCode.UnAuthorized);
                unAuthorizedApiResponse.SetErrorCode((int)ApiStatusCode.UnAuthorized);
                var objectResult = new ObjectResult(unAuthorizedApiResponse);
                objectResult.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = objectResult;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
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
