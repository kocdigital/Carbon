using System;

namespace Carbon.TimeSeriesDb.Abstractions.Entities
{

    public abstract class TimeSerieEntityBase : ITimeSeriesEntity
    {
        public Guid Id { get; set; }
    }

}
