using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 定义权限 ABP会自动发现这个类,不需要在管道里面进行配置!
    /// </summary>
    public class SectionPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(SectionPermissions.GroupName, L("Sections"));

            //你需要在 Define 方法中添加权限组或者获取已存在的权限组,并向权限组中添加权限.
            //在界面上配置用户权限保存后更改会持久化到数据库并在授权系统使用.
            //Define your own permissions here. Example:
            //myGroup.AddPermission(NetCorePermissions.MyPermission1, L("Permission:MyPermission1"));//本地化权限名称

            myGroup.AddPermission(SectionPermissions.Sections.Default, L("Sections.Default"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.Create, L("Sections.Create"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.Update, L("Sections.Update"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.Delete, L("Sections.Delete"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.Worker, L("Sections.Worker"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.WorkerDetail, L("Sections.WorkerDetail"));//本地化权限名称
            myGroup.AddPermission(SectionPermissions.Sections.Report, L("Sections.Report"));//本地化权限名称
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}
