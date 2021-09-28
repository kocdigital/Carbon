using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Carbon.HttpClient.Auth.IdentityServerSupport
{
    public class AuthHttpClientFactoryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthHttpClientFactory _authHttpClientFactory;
        public AuthHttpClientFactoryMiddleware(RequestDelegate next, AuthHttpClientFactory authHttpClientFactory)
        {
            _next = next;
            _authHttpClientFactory = authHttpClientFactory;
        }
        public Task Invoke(HttpContext httpContext)
        {
            _authHttpClientFactory.CurrentSessionHeaders = new Dictionary<string, string>();
             var headers = httpContext.Request.Headers;
            List<string> authNames = _authHttpClientFactory.GetAuthNames();
            foreach (var hdr in headers)
            {
                
                var authname = authNames.FirstOrDefault(k => hdr.Key.Contains(k));
                if (authname != null && hdr.Key.Contains("_"))
                {
                    var hdrSplitted = hdr.Key.Split('_');
                    hdrSplitted[0] = String.Empty;
                    var hdrJoined = String.Join(String.Empty, hdrSplitted);
                    _authHttpClientFactory.CurrentSessionHeaders.Add(hdrJoined, hdr.Value);
                }
            }
            return _next(httpContext);
        }
    }
}
