using AbpCore.OTel.Roles.Dto;
using System.Collections.Generic;

namespace AbpCore.OTel.Web.Models.Roles;

public class RoleListViewModel
{
    public IReadOnlyList<PermissionDto> Permissions { get; set; }
}
