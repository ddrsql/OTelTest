using System.Collections.Generic;
using AbpFramework.OTel.Roles.Dto;
using AbpFramework.OTel.Users.Dto;

namespace AbpFramework.OTel.WebMpa.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}