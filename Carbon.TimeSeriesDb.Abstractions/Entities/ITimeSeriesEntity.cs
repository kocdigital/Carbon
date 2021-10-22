using System;

namespace Carbon.TimeSeriesDb.Abstractions.Entities
{

    public interface ITimeSeriesEntity
    {
        public Guid Id { get; set; }
    }

}
