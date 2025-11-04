using Abp.Authorization;
using AbpFramework.OTel.Authorization.Roles;
using AbpFramework.OTel.Authorization.Users;

namespace AbpFramework.OTel.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
