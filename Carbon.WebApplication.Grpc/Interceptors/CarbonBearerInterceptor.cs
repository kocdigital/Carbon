using Carbon.WebApplication.Middlewares;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Carbon.WebApplication.Grpc.Interceptors
{
	public class CarbonBearerInterceptor : Interceptor
	{
		public CarbonBearerInterceptor()
		{

		}
		public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
		{
            var bearerToken = context.RequestHeaders.Get("authorization")?.Value ?? context.RequestHeaders.Get("Authorization")?.Value;
            bool isGodUser = false;
            if (!String.IsNullOrEmpty(bearerToken))
            {
                if (bearerToken != null)
                {
                    var rawToken = bearerToken.Replace($"Bearer ", "");
                    var securityToken = new JwtSecurityToken(rawToken);

                    var goduserHeader = context.RequestHeaders.Where(k => k.Key.ToLower() == "goduser" || k.Key == "god-user").FirstOrDefault();
                    if (goduserHeader != null)
                    {
                        if (Boolean.TryParse(goduserHeader.Value, out isGodUser))
                        {
                            context.RequestHeaders.Remove(goduserHeader);
                        }
                    }

                    if (securityToken.Claims == null || !securityToken.Claims.Where(k => k.Type == "god-user" && k.Value == "true").Any())
                    {
                        var tenantIdHeader = context.RequestHeaders.Where(k => k.Key.ToLower() == "tenantid" || k.Key.ToLower() == "tenant-id").FirstOrDefault();
                        if (tenantIdHeader != null)
                        {
                            context.RequestHeaders.Remove(tenantIdHeader);
                        }
                    }

                    var clientidHeader = context.RequestHeaders.Where(k => k.Key.ToLower() == "clientid").FirstOrDefault();
                    if (clientidHeader != null)
                    {
                        context.RequestHeaders.Remove(clientidHeader);
                    }

                    if (securityToken.Claims != null)
                    {
                        foreach (var claim in securityToken.Claims)
                        {
                            if (BearerTokenClaimMapper.TryGetValue(claim.Type, out string mappedKey))
                            {
                                context.RequestHeaders.Add(mappedKey, claim.Value);
                            }
                        }
                    }
                }
            }

            if (!isGodUser)
            {
                var _type = request.GetType();
                PropertyInfo prop = null;
                if ((prop = _type.GetProperty("TenantId")) != null || (prop = _type.GetProperty("tenantid")) != null)
                {
                    var tenantId = context.RequestHeaders.Get("TenantId")?.Value ?? context.RequestHeaders.Get("tenantid")?.Value;
                    if (string.IsNullOrWhiteSpace(tenantId))
                        throw new ArgumentNullException("TenantId");

                    prop.SetValue(request, tenantId);
                }
            }
            return base.UnaryServerHandler(request, context, continuation);
        }
	}
}
