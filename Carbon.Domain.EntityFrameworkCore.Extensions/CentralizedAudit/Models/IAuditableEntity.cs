using System;
using System.ComponentModel.DataAnnotations;

namespace Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Models
{
    public interface IAuditableEntity<T> where T : IEquatable<T>
    {
        [Key] T Id { get; }

        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}
