using Carbon.Domain.Abstractions.Entities;
using Carbon.TimeSeriesDb.Abstractions.Attributes;
using Carbon.TimeSeriesDb.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.TimeScaleDb.EntityFrameworkCore
{
    public abstract class TimeSeriesEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : TimeSerieEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TBase> entityTypeBuilder)
        {
            var tsField = entityTypeBuilder.Metadata.ClrType.GetProperties().Where(k => k.CustomAttributes.Where(k => k.AttributeType == typeof(TimeSerie)).Any()).ToList();
            if (tsField == null || !tsField.Any())
            {
                throw new NotSupportedException("Database object does not contain any timeserie field! Remember to tag any of your DateTime Property with [TimeSerie] Attribute");
            }
            else if (tsField.Count > 1)
            {
                throw new NotImplementedException("Database object contains more than 1 timeserie field! Remember to tag only one of your DateTime Property with [TimeSerie] Attribute");
            }

            entityTypeBuilder.HasKey(tsField[0].Name, "Id");
            entityTypeBuilder.Property("Id").ValueGeneratedOnAdd();
        }
    }

}
