using Carbon.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.HttpClients;
using System.Net.Http;
using System.Collections.Generic;
using Carbon.Common;

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
            services.AddHttpClientWithHeaderPropagation((x) =>
            {

            });

            var values = new List<ErrorCode>
            {
                new ErrorCode(100,  "ErrorCode1"),
                new ErrorCode(101,  "ErrorCode2"),
                new ErrorCode(102,  "ErrorCode3"),
                new ErrorCode(103,  "ErrorCode4"),
                new ErrorCode(104,  "ErrorCode5"),
            };

            services.AddErrorCodes(values);

            //services.AddHttpClientWithZipkinTracing(c =>
            //{
            //    c.DefaultRequestHeaders.Add("CustomBiHeader", "CustomBiValue");
            //    c.BaseAddress = new System.Uri("http://custombaseaddres.com");
            //}, Environment, "OneM2MClient");

            // services.AddRedisPersister(Configuration);
        }

        public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseZipkin(env, Configuration);
        }
    }
}
