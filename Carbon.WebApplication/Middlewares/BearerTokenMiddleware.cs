using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Carbon.WebApplication.Middlewares
{

    public class BearerTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private const string BearerHeaderName = "Bearer";

        public BearerTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var authorizationTokens = httpContext.Request.Headers[HeaderNames.Authorization];

            if (authorizationTokens.Count > 0)
            {
                var bearerToken = authorizationTokens.FirstOrDefault(x => x.Contains(BearerHeaderName));

                if (bearerToken != null)
                {
                    httpContext.Request.Headers.Remove("TenantId");
                    httpContext.Request.Headers.Remove("ClientId");

                    var rawToken = bearerToken.Replace($"{BearerHeaderName} ", "");
                    var securityToken = new JwtSecurityToken(rawToken);

                    if (securityToken.Claims != null)
                    {
                        foreach (var claim in securityToken.Claims)
                        {
                            if(BearerTokenClaimMapper.TryGetValue(claim.Type, out string mappedKey))
                            {
                                httpContext.Request.Headers.TryAdd(mappedKey, claim.Value);
                            }
                        }

                    }
                }

            }

            return _next(httpContext);
        }
    }
}
