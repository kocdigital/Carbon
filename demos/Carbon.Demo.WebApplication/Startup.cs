using Carbon.WebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Demo.WebApplication
{
    public class Startup : CarbonStartup<Startup>
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureDependencies(IServiceCollection services)
        {

        }

        public override void ConfigureRequestPipeline(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
