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
                _authHttpClientFactory = scope.ServiceProvider.GetRequiredService<AuthHttpClientFactory>();
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            //Token Validation will appear here
            var daRes = _authHttpClientFactory.GetAuthentication(GrpcClientStaticValues.GrpcAuthenticator).Result;
            Metadata newMetadata = new Metadata();
            newMetadata.Add("Authorization", "Bearer " + daRes.PrimaryAccessId);

            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method,
                 context.Host, context.Options.WithHeaders(newMetadata));

            var response = base.AsyncUnaryCall(request, context, continuation);
            try
            {
                var resResponse = response.ResponseAsync.Result;
                return response;
            }
            catch (Exception exc)
            {
                var ex = exc.InnerException as RpcException;
                if (ex != null && ex.StatusCode == StatusCode.Unauthenticated)
                {
                    var reAuth = _authHttpClientFactory.ReAuthenticate(GrpcClientStaticValues.GrpcAuthenticator).Result;
                    if (reAuth == null)
                        return response;

                    Metadata newMetadataReAuthed = new Metadata();
                    newMetadataReAuthed.Add("Authorization", "Bearer " + reAuth.PrimaryAccessId);
                    context = new ClientInterceptorContext<TRequest, TResponse>(context.Method,
                        context.Host, context.Options.WithHeaders(newMetadataReAuthed));

                    return base.AsyncUnaryCall(request, context, continuation);
                }
                else
                {
                    return response;
                }
            }



        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.BlockingUnaryCall(request, context, continuation);
        }
    }
}