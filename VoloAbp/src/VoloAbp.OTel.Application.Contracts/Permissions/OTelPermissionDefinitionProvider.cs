using VoloAbp.OTel.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace VoloAbp.OTel.Permissions;

public class OTelPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var otelGroup = context.AddGroup(OTelPermissions.GroupName, L("Permission:BookStore"));

        var booksPermission = otelGroup.AddPermission(OTelPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(OTelPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(OTelPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(OTelPermissions.Books.Delete, L("Permission:Books.Delete"));

        var authorsPermission = otelGroup.AddPermission(
            OTelPermissions.Authors.Default, L("Permission:Authors"));
        authorsPermission.AddChild(
            OTelPermissions.Authors.Create, L("Permission:Authors.Create"));
        authorsPermission.AddChild(
            OTelPermissions.Authors.Edit, L("Permission:Authors.Edit"));
        authorsPermission.AddChild(
            OTelPermissions.Authors.Delete, L("Permission:Authors.Delete"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(OTelPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OTelResource>(name);
    }
}
