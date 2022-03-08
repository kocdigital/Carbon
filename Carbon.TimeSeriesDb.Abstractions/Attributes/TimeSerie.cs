using System;
using System.ComponentModel.DataAnnotations;

namespace Carbon.TimeSeriesDb.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class TimeSerie : ValidationAttribute
    {

    }
}
