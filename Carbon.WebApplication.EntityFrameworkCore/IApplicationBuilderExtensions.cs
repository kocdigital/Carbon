using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Carbon.WebApplication.EntityFrameworkCore
{
    public static class IApplicationBuilderExtensions
    {
        public static void MigrateDatabase<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<TContext>().Database.Migrate();
                var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

                context.Database.Migrate();
            }
        }
    }
}
