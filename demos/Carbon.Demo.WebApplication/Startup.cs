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

        public override void ConfigureDependencies(IServiceCollection services)
        {
            services.AddHttpClientWithHeaderPropagation(x =>
            {

            });
            services.AddRedisPersister(Configuration);
        }

        public override void ConfigureRequestPipeline(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
