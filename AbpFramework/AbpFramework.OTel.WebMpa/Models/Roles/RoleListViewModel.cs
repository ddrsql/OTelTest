using System.Collections.Generic;
using AbpFramework.OTel.Roles.Dto;

namespace AbpFramework.OTel.WebMpa.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}