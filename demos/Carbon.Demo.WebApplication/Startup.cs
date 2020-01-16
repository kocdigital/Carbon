using Carbon.Demo.WebApplication.Application.Dtos;
using Carbon.WebApplication;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.Demo.WebApplication
{
    public class Startup : CarbonStartup<Startup>
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
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
