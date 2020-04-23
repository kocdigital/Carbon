using Carbon.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.HttpClients;
using Carbon.Redis;

namespace Carbon.Demo.WebApplication
{
    public class Startup : CarbonStartup<Startup>
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
        {
            AddOperationFilter<ComposerHeaderFilter>();
        }

        public override void CustomConfigureServices(IServiceCollection services)
        {
            services.AddHttpClientWithHeaderPropagation(x =>
            {

            });
            services.AddRedisPersister(Configuration);
        }

        public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
