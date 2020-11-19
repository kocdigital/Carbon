using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.EntityFrameworkCore
{
    public abstract class TenantManagedEntityConfigurationBase<T> : IEntityTypeConfiguration<T>
        where T : class, IEntity, IHaveOwnership<EntitySolutionRelation>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Ignore(x => x.RelationalOwners);
        }
    }
}
