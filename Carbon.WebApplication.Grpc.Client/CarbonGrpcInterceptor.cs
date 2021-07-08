using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Linq;

namespace Carbon.WebApplication.Grpc.Client
{
    internal class CarbonGrpcInterceptor : Interceptor
    {
        public CarbonGrpcInterceptor()
        {

        }
        //static string token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjJDNzBDREQyNUVDNTRGQUZEODkzRkMyMTYwMkJBMDdGQUZEQTU1NDlSUzI1NiIsInR5cCI6ImF0K2p3dCIsIng1dCI6IkxIRE4wbDdGVDZfWWtfd2hZQ3VnZjZfYVZVayJ9.eyJuYmYiOjE2MjQ5OTEwODQsImV4cCI6MTYyNTI5MTA4NCwiaXNzIjoiaHR0cHM6Ly9wbGF0Zm9ybTM2MGlkZW50aXR5LmtvY2RpZ2l0YWwuY29tIiwiYXVkIjpbIlBsYXRmb3JtMzYwLkNvbW1vbi5BUEkiLCJQbGF0Zm9ybTM2MC5UZW5hbnRzLkFQSSIsIlBsYXRmb3JtMzYwLlByb2R1Y3RzLkFQSSIsIlBsYXRmb3JtMzYwLkF6dXJlRW5hYmxlbWVudC5BUEkiLCJQbGF0Zm9ybTM2MC5Qb2xpY3lTZXJ2ZXIuQVBJIiwiUGxhdGZvcm0zNjAuSWRlbnRpdHlTZXJ2ZXIuVXNlcnMuQVBJIiwiUGxhdGZvcm0zNjAuSWRlbnRpdHlTZXJ2ZXIuUmVzb3VyY2VNYW5hZ2VtZW50LkFQSSJdLCJjbGllbnRfaWQiOiJyYW1jbGllbnQiLCJzdWIiOiJkY2I2NjUwNi03YzJjLTQ0MGMtYjNhMi0wOGQ5MmZjNmEwZTEiLCJhdXRoX3RpbWUiOjE2MjQ5OTEwODMsImlkcCI6ImxvY2FsIiwibmFtZSI6Im1leHRhZG1pbnVzZXIiLCJlbWFpbCI6Im1leHRhZG1pbnVzZXJAbWV4dC5jb20udHIiLCJvcmdhbml6YXRpb24taWQiOiI4N2VkMmQzZi00Yjg0LTQ2MDAtYThiNy0wOGQ5MmZjNmEwNjEiLCJ0ZW5hbnQtaWQiOiJhOWFhZTdkMi0xN2JkLTQ2Y2YtYzcwOC0wOGQ5MmZjNmEwNTYiLCJzaWQiOiI3QkY4RkMzQzA5N0U3QUQ1OTU3NDc5ODAwRUE2RTlGOSIsImlhdCI6MTYyNDk5MTA4NCwic2NvcGUiOlsiUGxhdGZvcm0zNjAuQ29tbW9uLkFQSSIsIlBsYXRmb3JtMzYwLlRlbmFudHMuQVBJIiwiUGxhdGZvcm0zNjAuUHJvZHVjdHMuQVBJIiwiUGxhdGZvcm0zNjAuQXp1cmVFbmFibGVtZW50LkFQSSIsIlBsYXRmb3JtMzYwLlBvbGljeVNlcnZlci5BUEkiLCJQbGF0Zm9ybTM2MC5JZGVudGl0eVNlcnZlci5Vc2Vycy5BUEkiLCJQbGF0Zm9ybTM2MC5JZGVudGl0eVNlcnZlci5SZXNvdXJjZU1hbmFnZW1lbnQuQVBJIl0sImFtciI6WyJwd2QiXX0.CmuuJ8HVK5Es2vV0h7IoFvuVwlnMgITj8RvBTYtH1IV_tXb-tgggdjlgZ_A0xCok0mLNbLAhMXeB4iIIeDqvY8unxl1Y1Mqzx5rJW5an_Dy2OmGFx63vNjEGxUpqLEPRwMHnGQ2qCbrkTiYZGaLEu0MkU_KOCuuWdSQytdYqN_A85ZYCHUH0dXZjM9T2Rft17ugGLuP821YPMwcpB909Ujslp7q8axmyjWUZh4SBnTxcFa23twzOhWbwiuTeAfOthnbAG0GeJzt2LWBEMTAdPjJUbZrAgGAwbFhc-Ly6FUZPvDmkOM_kQdbP91I3mUagg4JT6R4L9zATqL5kVAL9hw";

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