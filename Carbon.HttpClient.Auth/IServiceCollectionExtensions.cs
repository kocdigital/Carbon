using Carbon.HttpClient.Auth.IdentityServerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.HttpClient.Auth
{
    public static class GrpcAuthClientSupport
    {
        public static bool Enabled = false;
    }

    /// <summary>
	/// Contains extension methos like AddHttpClientAuth, CreateAuthentication etc. for <see cref="IServiceCollection"/> and <see cref="IApplicationBuilder"/>
	/// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds a http client support to call identity server-supported APIs
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpClientAuth(this IServiceCollection services)
        {
            services.AddScoped<AuthHttpClientFactory>();
            services.AddSingleton<AuthHttpClientAuthorization>();
        }

        /// <summary>
        /// Adds a http client support to call identity server-supported APIs
        /// </summary>
        /// <param name="services"></param>
        public static void AddGrpcHttpClientAuth(this IServiceCollection services)
        {
            GrpcAuthClientSupport.Enabled = true;
            services.AddScoped<AuthHttpClientFactory>();
            services.AddSingleton<AuthHttpClientAuthorization>();
        }

        /// <summary>
        /// Creates authenticated http client to call identity server-supported APIs with the given name in Configuration
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name">HttpClient Name</param>
        /// <returns></returns>
        public static IApplicationBuilder CreateAuthentication(this IApplicationBuilder app, string name)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetRequiredService<AuthHttpClientFactory>();
                var result = service.CreateAuthentication(name).Result;
                Console.WriteLine($"Authentication Created ==> {result.Name} with Token {result.PrimaryAccessId}");
            }

            return app;
        }


    }
}
