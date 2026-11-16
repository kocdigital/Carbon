using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Carbon.WebApplication.Grpc.Client
{
	internal class CarbonGrpcInterceptor : Interceptor
	{
		public CarbonGrpcInterceptor()
		{

		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			//Token Validation will appear here

			//Metadata newMetadata = new Metadata();
			//newMetadata.Add("Authorization", "Bearer " + token);
			//context = new ClientInterceptorContext<TRequest, TResponse>(context.Method,
			//     context.Host, context.Options.WithHeaders(newMetadata));

			return base.AsyncUnaryCall(request, context, continuation);
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			return base.BlockingUnaryCall(request, context, continuation);
		}
	}
}