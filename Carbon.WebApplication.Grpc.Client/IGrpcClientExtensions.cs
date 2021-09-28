using Carbon.HttpClient.Auth;
using Carbon.HttpClient.Auth.IdentityServerSupport;
using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Carbon.WebApplication.Grpc.Client
{
    public static class IGrpcClientExtensions
    {
        /// <summary>
        /// Adds Carbon Grpc Client Support. (Use this as latest in your service dependency registration)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="grpcServerAddress"></param>
        public static void AddCarbonGrpcClient<T>(this IServiceCollection services, string grpcServerAddress) where T : ClientBase
        {
            if (!GrpcAuthClientSupport.Enabled)
                services.AddGrpcClient<T>(o =>
                {
                    o.Address = new Uri(grpcServerAddress);
                }).AddInterceptor(() => new CarbonGrpcInterceptor());
            else
                services.AddGrpcClient<T>(o =>
                {
                    o.Address = new Uri(grpcServerAddress);
                }).AddInterceptor((prv) => new CarbonGrpcAuthInterceptor(prv));
        }
    }
}
