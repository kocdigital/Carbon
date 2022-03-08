using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.TimeScaleDb.EntityFrameworkCore
{
    /// <summary>
    ///     A carbon wrapper class for database context objects.
    /// </summary>
    /// <typeparam name="TContext"> A database context object to be wrapped. </typeparam>
    public class CarbonTimeScaleDbReadOnlyContext<TContext> : CarbonTimeScaleDbContext<TContext>, ITimeScaleDbReadOnlyContext where TContext : DbContext
    {
        /// <summary>
        ///     Constructor that initializes the CarbonContext with the given options for database context
        /// </summary>
        /// <param name="options"> Options for DbContext constructor </param>
        public CarbonTimeScaleDbReadOnlyContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder
        .UseNpgsql().UseLowerCaseNamingConvention(System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Set to true to discard tracked changes after the save operation ends in success.</param>
        /// <returns>Number of state entries written to the database.</returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <remarks> 
        ///     Multiple SaveChangesAsync operations on the same context is not supported, therefore it is advised to call
        ///     use the "await" keyword to let the task finish before any other operations on this context.
        /// </remarks>
        /// <param name="acceptAllChangesOnSuccess">Set to true to discard tracked changes after the save operation ends in success.</param>
        /// <returns>A task with number of state entries written to the database as its result.</returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new InvalidOperationException("This context is read-only.");
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("This context is read-only.");
        }

        public override int SaveChanges()
        {
            throw new InvalidOperationException("This context is read-only.");
        }

    }
}
