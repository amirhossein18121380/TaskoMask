﻿using System.Threading.Tasks;
using TaskoMask.Domain.Core.Data;
using TaskoMask.Domain.WriteModel.Membership.Entities;
using System.Collections.Generic;

namespace TaskoMask.Domain.WriteModel.Membership.Data
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<bool> ExistByNameAsync(string id, string name);
        Task<IEnumerable<Role>> GetListByIdsAsync( string[] selectedRolesId);
        Task<IEnumerable<Role>> GetListByPermissionIdAsync(string permissionId);
        Task<long> CountByPermissionIdAsync(string permissionId);

    }
}