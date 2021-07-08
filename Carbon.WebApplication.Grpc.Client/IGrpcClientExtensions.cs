using Microsoft.Extensions.DependencyInjection;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
