1- Add Package References
	Carbon.Common
	Carbon.ExceptionHandling.Abstractions
	Carbon.WebApplication
	Carbon.Domain.Abstractions
2- Create an Application Folder 
	Put default Controllers' folder into Application Folder
3- Create a Domain Folder
4- Create a Infrastructure Folder
5- Change Program.cs with the code below

    public class Program : CarbonProgram<Startup>
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }

6- Change Startup.cs with the code below

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

7- 