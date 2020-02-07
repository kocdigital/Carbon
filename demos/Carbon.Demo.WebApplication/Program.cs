using Carbon.WebApplication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Carbon.Demo.WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseCarbonFeatures();
            webBuilder.UseStartup<Startup>();
        });
    }
}
