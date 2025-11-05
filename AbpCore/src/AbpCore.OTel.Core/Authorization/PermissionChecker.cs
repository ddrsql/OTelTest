using Abp.Authorization;
using AbpCore.OTel.Authorization.Roles;
using AbpCore.OTel.Authorization.Users;

namespace AbpCore.OTel.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
