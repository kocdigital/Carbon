using System;
using System.ComponentModel.DataAnnotations;

namespace Carbon.Domain.Abstractions.Attributes
{
    /// <summary>
    /// Checks related entities; if related entity is not deleted or soft deleted, throws <see cref="InvalidOperationException"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SoftDeleteConstraint : ValidationAttribute
    {

    }
}
