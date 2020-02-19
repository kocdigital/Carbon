using Carbon.ConsoleApplication;
using System.Threading.Tasks;

namespace Carbon.Demo.ConsoleApplication
{
    class Program
    {
        public static async Task Main()
        {
            await HostManager.RunAsync<Program>();

            System.Console.WriteLine("Hello World");
        }
    }
}
