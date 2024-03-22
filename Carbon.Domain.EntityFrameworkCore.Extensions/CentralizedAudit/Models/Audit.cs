using System;

namespace Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Models
{
    public class Audit
    {
        public Guid Id { get; set; }
        public string ModifiedBy { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AuditType { get; set; }
    }
}