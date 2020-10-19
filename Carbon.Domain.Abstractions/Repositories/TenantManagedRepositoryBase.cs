using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Common.TenantManagementHandler.Interfaces;
using Carbon.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.Repositories
{
    public abstract class TenantManagedRepositoryBase : ISolutionFilteredRepository, IOwnershipFilteredRepository
    {
        public List<Guid> FilterSolutionList { get; set; }
        public List<PermissionDetailedDto> FilterOwnershipList { get; set; }

        public TenantManagedRepositoryBase()
        {

        }

        public void SetSolutionFilter(List<Guid> filters)
        {
            this.FilterSolutionList = filters;
        }

        public void SetOwnershipFilter(List<PermissionDetailedDto> filters)
        {
            this.FilterOwnershipList = filters;
        }

        public abstract Task ConnectToSolution<T>(T relatedEntity)
            where T : class, IHaveOwnership<EntitySolutionRelation>, IEntity;

        public abstract Task RemoveSolutions<T>(T relatedEntity)
            where T : class, IHaveOwnership<EntitySolutionRelation>, IEntity;

    }
}
