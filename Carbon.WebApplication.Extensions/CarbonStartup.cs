using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;

namespace Carbon.WebApplication.Extensions
{
    public abstract class CarbonStartup<T>
    {
        protected CarbonStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc().AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<T>());

            return services.BuildServiceProvider();
        }
    }
