using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.Common.TenantManagementHandler.Interfaces;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;

namespace Carbon.WebApplication.TenantManagementHandler.Abstracts
{
    /// <summary>
	/// Base class for services which contains solution and ownership based operations via repositories
	/// </summary>
    public abstract class TenantManagedServiceBase : ISolutionFilteredService, IOwnershipFilteredService
    {
        List<ISolutionFilteredRepository> _solutionFilteredRepositories { get; set; }
        List<IOwnershipFilteredRepository> _ownershipFilteredRepositories { get; set; }
        public TenantManagedServiceBase(List<ISolutionFilteredRepository> solutionFilteredRepositories, List<IOwnershipFilteredRepository> ownershipFilteredRepositories)
        {
            _solutionFilteredRepositories = solutionFilteredRepositories;
            _ownershipFilteredRepositories = ownershipFilteredRepositories;
        }
        public List<Guid> FilterSolutionList { get; set; }
        public List<PermissionDetailedDto> FilterOwnershipList { get; set; }

        /// <summary>
        /// Adds solution filtes
        /// </summary>
        /// <param name="filters">Solution Ids</param>
        public void SetFilter(List<Guid> filters)
        {
            this.FilterSolutionList = filters;

            foreach (var solutionFilteredRepo in _solutionFilteredRepositories)
            {
                solutionFilteredRepo.SetSolutionFilter(filters);
            }
        }

        /// <summary>
		/// Adds ownership filtes
		/// </summary>
		/// <param name="filters">Contains information about Permission and it's policies. See: <see cref="PermissionDetailedDto"/></param>
        public void SetFilter(List<PermissionDetailedDto> filters)
        {
            this.FilterOwnershipList = filters;

            foreach (var ownershipFilteredRepo in _ownershipFilteredRepositories)
            {
                ownershipFilteredRepo.SetOwnershipFilter(filters);
            }
        }
    }
}
