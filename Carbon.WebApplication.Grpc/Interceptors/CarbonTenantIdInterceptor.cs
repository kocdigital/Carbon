using Grpc.Core;
using Grpc.Core.Interceptors;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Carbon.WebApplication.Grpc.Interceptors
{
	public class CarbonTenantIdInterceptor : Interceptor
	{
		public CarbonTenantIdInterceptor()
		{

		}
		public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
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
			return base.UnaryServerHandler(request, context, continuation);
		}
	}
}
