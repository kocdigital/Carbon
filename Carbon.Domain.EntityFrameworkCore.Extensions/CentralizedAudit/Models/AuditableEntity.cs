using System;
using System.ComponentModel.DataAnnotations;

namespace Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Models
{
    public class AuditableEntity : IAuditableEntity<Guid>
    {
        protected AuditableEntity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        [MaxLength(256)]
        public string CreatedBy { get; set; }

        [MaxLength(256)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
