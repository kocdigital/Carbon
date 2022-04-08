using Carbon.Domain.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Carbon.Quartz.Migrate.Context
{
    public class QuartzMigrationContext : CarbonContext<QuartzMigrationContext>
    {
        public QuartzMigrationContext(DbContextOptions<QuartzMigrationContext> options) : base(options)
        {

        }

    }

}
