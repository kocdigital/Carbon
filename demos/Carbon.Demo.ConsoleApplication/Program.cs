using Carbon.ConsoleApplication;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Carbon.Demo.ConsoleApplication
{
    class Program
    {
        public static async Task Main()
        {
            await new HostBuilder().UseCarbonFeatures<Program>().RunConsoleAsync();
        }
    }
}
