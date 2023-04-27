using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace Carbon.Common
{
    public static class HttpRequestExtensions
    {

        /// <summary>
        /// Gets CorrelationId from the header
        /// </summary>
        /// <returns>CorrelationId</returns>
        public static StringValues GetIdentifier(this HttpRequest Request)
        {
            if (Request != null && Request.Headers != null)
            {
                if (Request.Headers.TryGetValue("X-CorrelationId", out var xCorrelationId))
                {
                    return xCorrelationId;
                }
                else if (Request.Headers.TryGetValue("correlationId", out var correlationId))
                {
                    return correlationId;
                }
            }
            
            return Guid.NewGuid().ToString();
        }
    }
}
