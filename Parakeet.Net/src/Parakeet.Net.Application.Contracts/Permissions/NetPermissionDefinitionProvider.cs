using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions;

public class NetPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NetPermissions.GroupName, L("Net"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(NetPermissions.MyPermission1, L("Permission:MyPermission1"));

        //你需要在 Define 方法中添加权限组或者获取已存在的权限组,并向权限组中添加权限.
        //保存后更改会持久化到数据库并在授权系统使用.
        //Define your own permissions here. Example:
        //myGroup.AddPermission(NetPermissions.MyPermission1, L("Permission:MyPermission1"));//本地化权限名称

        myGroup.AddPermission(NetPermissions.Net.Default, L("Net.Default"));//本地化权限名称
        myGroup.AddPermission(NetPermissions.Net.Create, L("Net.Create"));//本地化权限名称
        myGroup.AddPermission(NetPermissions.Net.Update, L("Net.Update"));//本地化权限名称
        myGroup.AddPermission(NetPermissions.Net.Delete, L("Net.Delete"));//本地化权限名称

        //myGroup.AddPermission(
        //    "BookStore_Author_Create",
        //    LocalizableString.Create<NetResource>("Permission:BookStore_Author_Create"),
        //    multiTenancySide: MultiTenancySides.Tenant //多租户!如果你的应用程序不是多租户的,可以忽略这个选项.
        //);

        ////一个权限可能含有子权限,并且子权限只有在授权父权限时才可用.
        //var authorManagement = myGroup.AddPermission("Author_Management");
        //authorManagement.AddChild("Author_Management_Create_Books");
        //authorManagement.AddChild("Author_Management_Edit_Books");
        //authorManagement.AddChild("Author_Management_Delete_Books");

        //找role的delete权限，禁用它，因此没有人可以在应用程序中删除角色.
        context.GetPermissionOrNull(IdentityPermissions.Roles.Delete).IsEnabled = false;
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NetResource>(name);
    }
}
