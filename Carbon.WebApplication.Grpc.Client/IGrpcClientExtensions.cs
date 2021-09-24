using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Carbon.WebApplication.Grpc.Client
{
	public static class IGrpcClientExtensions
	{
		public static void AddCarbonGrpcClient<T>(this IServiceCollection services, string grpcServerAddress) where T : ClientBase
		{
			services.AddGrpcClient<T>(o =>
			{
				o.Address = new Uri(grpcServerAddress);
			}).AddInterceptor(() => new CarbonGrpcInterceptor());
		}
	}
}
