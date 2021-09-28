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
    public class CarbonRequestDtoInterceptor : Interceptor
    {
        public CarbonRequestDtoInterceptor()
        {

        }
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            bool isGodUser = false;

            var goduserHeader = context.RequestHeaders.Where(k => k.Key.ToLower() == "goduser" || k.Key == "god-user").FirstOrDefault();
            if (goduserHeader != null)
            {
                Boolean.TryParse(goduserHeader.Value, out isGodUser);
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
