﻿using TaskoMask.Domain.Workspace.Organizations.Data;
using TaskoMask.Domain.Workspace.Organizations.Services;

namespace TaskoMask.Infrastructure.Data.Services
{

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationValidatorService : IOrganizationValidatorService
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationValidatorService(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool OrganizationHasUniqueName(string id, string ownerOwnerId, string name)
        {
            return _organizationRepository.ExistByName(id, ownerOwnerId, name);
        }
    }
}