using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 定义权限 ABP会自动发现这个类,不需要在管道里面进行配置!
    /// </summary>
    public class NeedPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(NeedPermissions.GroupName, L("Needs"));

            //你需要在 Define 方法中添加权限组或者获取已存在的权限组,并向权限组中添加权限.
            //在界面上配置用户权限保存后更改会持久化到数据库并在授权系统使用.
            //Define your own permissions here. Example:
            //myGroup.AddPermission(NetPermissions.MyPermission1, L("Permission:MyPermission1"));//本地化权限名称

            myGroup.AddPermission(NeedPermissions.Needs.Default, L("Needs.Default"));//本地化权限名称
            myGroup.AddPermission(NeedPermissions.Needs.Create, L("Needs.Create"));//本地化权限名称
            myGroup.AddPermission(NeedPermissions.Needs.Update, L("Needs.Update"));//本地化权限名称
            myGroup.AddPermission(NeedPermissions.Needs.Delete, L("Needs.Delete"));//本地化权限名称
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}
