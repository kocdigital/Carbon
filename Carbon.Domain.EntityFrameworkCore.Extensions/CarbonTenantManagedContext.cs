using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    /// <summary>
    ///     A carbon wrapper class for database context objects.
    /// </summary>
    /// <typeparam name="TContext"> A database context object to be wrapped. </typeparam>
    public class CarbonTenantManagedContext<TContext> : CarbonContext<TContext> where TContext : DbContext
    {
        public CarbonTenantManagedContext():base()
        {
            
        }
        /// <summary>
        ///     Constructor that initializes the CarbonContext with the given options for database context
        /// </summary>
        /// <param name="options"> Options for DbContext constructor </param>
        public CarbonTenantManagedContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// EntitySolutionRelation Table
        /// </summary>
        public DbSet<EntitySolutionRelation> EntitySolutionRelation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntitySolutionRelationEntityConfiguration());
            base.OnModelCreating(modelBuilder);
        }

    }
}
