using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Domain.Abstractions.Entities
{
    public class EntitySolutionRelation : IEntity, ISoftDelete, IUpdateAuditing, IDeleteAuditing, IInsertAuditing, IOwnerRelation
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public int EntityCode { get; set; }
        public Guid SolutionId { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string DeletedUser { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string InsertedUser { get; set; }
        public DateTime? InsertedDate { get; set; }
    }
}
