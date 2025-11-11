using VoloAbp.OTel.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace VoloAbp.OTel.Permissions;

public class OTelPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(OTelPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(OTelPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OTelResource>(name);
    }
}
