using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Carbon.WebApplication
{
    public abstract class CarbonProgram<TStartup> where TStartup : class
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
    }
}
