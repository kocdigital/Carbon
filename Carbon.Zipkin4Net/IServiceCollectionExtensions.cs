using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using zipkin4net.Transport.Http;
using Carbon.HttpClients;
namespace Carbon.HttpClients
{
    public static class IServiceCollectionExtensions
    {
        public static void AddHttpClientWithZipkinTracing(this IServiceCollection services, Action<HttpClient> c, IWebHostEnvironment env, string clientName)
        {
            services.AddHttpClient(clientName, c).AddHttpMessageHandler(provider =>
                 TracingHandler.WithoutInnerHandler(env.ApplicationName));
        }


        public static void AddHttpClientWithZipkinTracing<T>(this IServiceCollection services, Action<HttpClient> c, IWebHostEnvironment env) where T : WebapiClient
        {
            services.AddHttpClient<T>(c).AddHttpMessageHandler(provider =>
                 TracingHandler.WithoutInnerHandler(env.ApplicationName));
        }
    }
}
