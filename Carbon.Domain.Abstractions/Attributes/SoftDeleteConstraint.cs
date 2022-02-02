using System;
using System.ComponentModel.DataAnnotations;

namespace Carbon.Domain.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SoftDeleteConstraint : ValidationAttribute
    {

    }
}
