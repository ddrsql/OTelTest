using Abp.AutoMapper;
using AbpCore.OTel.Roles.Dto;
using AbpCore.OTel.Web.Models.Common;

namespace AbpCore.OTel.Web.Models.Roles;

[AutoMapFrom(typeof(GetRoleForEditOutput))]
public class EditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
{
    public bool HasPermission(FlatPermissionDto permission)
    {
        return GrantedPermissionNames.Contains(permission.Name);
    }
}
