using Carbon.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.HttpClients;
using Carbon.Redis;
using Carbon.Zipkin4Net;
using System.Net.Http;

namespace Carbon.Demo.WebApplication
{
    public class Startup : CarbonStartup<Startup>
    {
        public class CustomWebApiClient:WebapiClient
        {
            public CustomWebApiClient(HttpClient c) : base(c)
            {

            }
        }


        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
        {
            AddOperationFilter<ComposerHeaderFilter>();
        }

        public override void CustomConfigureServices(IServiceCollection services)
        {


            services.AddHttpClientWithZipkinTracing(c =>
            {
                c.DefaultRequestHeaders.Add("CustomBiHeader", "CustomBiValue");
                c.BaseAddress = new System.Uri("http://custombaseaddres.com");
            }, Environment, "OneM2MClient");





            // services.AddRedisPersister(Configuration);
        }

        public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseZipkin(env, Configuration);
        }
    }
}
