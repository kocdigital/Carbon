using Carbon.HttpClient.Auth.IdentityServerSupport;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.WebApplication.Grpc.Client
{
    internal class CarbonGrpcAuthInterceptor : Interceptor
    {
        private readonly AuthHttpClientFactory _authHttpClientFactory;

        public CarbonGrpcAuthInterceptor(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                _authHttpClientFactory = serviceProvider.GetRequiredService<AuthHttpClientFactory>();
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            //Token Validation will appear here
            var daRes = _authHttpClientFactory.GetAuthentication("Grpc").Result;
            Metadata newMetadata = new Metadata();
            newMetadata.Add("Authorization", "Bearer " + daRes.PrimaryAccessId);
            
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method,
                 context.Host, context.Options.WithHeaders(newMetadata));

            return base.AsyncUnaryCall(request, context, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.BlockingUnaryCall(request, context, continuation);
        }
    }
}