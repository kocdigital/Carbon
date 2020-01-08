using Carbon.WebApplication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Carbon.Demo.WebApplication
{
    public class Program : CarbonProgram<Startup>
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
