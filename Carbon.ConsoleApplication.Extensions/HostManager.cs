using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Carbon.ConsoleApplication
{
    public static class HostManager
    {
        /// <summary>
        /// Creates and adds Carbon Features to <see cref="HostBuilder"/> and after Runs Console. Configures <see cref="HostBuilder"/> with given configurations
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="configureApp">Injects action for user defined configurations</param>
        /// <param name="configureServices">Injects action for user defined service configuraitons. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns><see cref="Task"/> which returns from <see cref="HostingHostBuilderExtensions.RunConsoleAsync"/></returns>
        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IConfigurationBuilder> configureApp, Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            await new HostBuilder().AddCarbonConfiguration<TProgram>(configureApp)
                                   .AddCarbonServices<TProgram>(configureServices)
                                   .RunConsoleAsync();
        }
        /// <summary>
        /// Creates and adds Carbon Features to <see cref="HostBuilder"/> and after Runs Console. Configures <see cref="HostBuilder"/> with given configurations
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="configureApp">Injects action for user defined configurations</param>
        /// <returns><see cref="Task"/> which returns from <see cref="HostingHostBuilderExtensions.RunConsoleAsync"/></returns>
        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IConfigurationBuilder> configureApp) where TProgram : class
        {
            await new HostBuilder().AddCarbonConfiguration<TProgram>(configureApp).RunConsoleAsync();
        }
        /// <summary>
        /// Creates and adds Carbon Features to <see cref="HostBuilder"/> and after Runs Console. Configures <see cref="HostBuilder"/> with given configurations
        /// </summary>
        /// <typeparam name="TProgram">Type of class which holds <see cref="IHostBuilder"/></typeparam>
        /// <param name="configureServices">Injects action for user defined service configuraitons. <see cref="IHostBuilder.ConfigureServices"/></param>
        /// <returns><see cref="Task"/> which returns from <see cref="HostingHostBuilderExtensions.RunConsoleAsync"/></returns>
        public static async Task RunAsync<TProgram>(Action<HostBuilderContext, IServiceCollection> configureServices) where TProgram : class
        {
            await new HostBuilder().AddCarbonServices<TProgram>(configureServices).RunConsoleAsync();
        }

    }
}
