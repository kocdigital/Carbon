using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlTypes;
using System.Net.Http;

namespace Carbon.HttpClients
{
    public class WebapiClient
    {
        public WebapiClient(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }
    }

    public static class IServiceCollectionExtensions
    {

        public static void AddHttpClientWithHeaderPropagation(this IServiceCollection services, Action<HeaderPropagationOptions> configureHeaderPropagation)
        {

            services.AddHeaderPropagation(o =>
            {
                o.Headers.Add("x-request-id", "6161613434343");
                o.Headers.Add("x-b3-traceid");
                o.Headers.Add("x-b3-spanid");
                o.Headers.Add("x-b3-parentspanid");
                o.Headers.Add("x-b3-sampled");
                o.Headers.Add("x-b3-flags");
                o.Headers.Add("x-ot-span-context");
            });

            services.AddHttpClient<WebapiClient>().AddHeaderPropagation(o => o.Headers.Add("x-request-id", "6161613434343"));
        }
    }
}
