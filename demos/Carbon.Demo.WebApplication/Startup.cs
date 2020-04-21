using Carbon.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Carbon.HttpClients;

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
        }

        public override void CustomConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
