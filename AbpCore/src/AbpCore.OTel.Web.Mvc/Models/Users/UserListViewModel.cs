using AbpCore.OTel.Roles.Dto;
using System.Collections.Generic;

namespace AbpCore.OTel.Web.Models.Users;

public class UserListViewModel
{
    public IReadOnlyList<RoleDto> Roles { get; set; }
}
