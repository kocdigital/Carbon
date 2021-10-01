using Carbon.Domain.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.EntityFrameworkCore
{
    public class EntitySolutionRelationEntityConfiguration : IEntityTypeConfiguration<EntitySolutionRelation>
    {
        public void Configure(EntityTypeBuilder<EntitySolutionRelation> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.EntityId)
                   .IsRequired(true);

            builder.Property(x => x.EntityCode)
                   .IsRequired(true);

            builder.Property(x => x.SolutionId)
                   .IsRequired(true);

            builder.HasIndex(k => new { k.EntityCode, k.EntityId, k.IsDeleted });
        }
    }
}
