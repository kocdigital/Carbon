using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication.TenantManagementHandler.Dtos
{
    public class EntitySolutionRelationDto
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public int EntityCode { get; set; }
        public Guid SolutionId { get; set; }
    }
}
