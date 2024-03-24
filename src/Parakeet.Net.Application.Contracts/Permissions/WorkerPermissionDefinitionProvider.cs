using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 工人权限
    /// </summary>
    public class WorkerPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(WorkerPermissions.GroupName,L("Workers"));

            myGroup.AddPermission(WorkerPermissions.Worker.Default, L("Workers.Default"));
            myGroup.AddPermission(WorkerPermissions.Worker.Create, L("Workers.Create"));
            myGroup.AddPermission(WorkerPermissions.Worker.Update, L("Workers.Update"));
            myGroup.AddPermission(WorkerPermissions.Worker.Delete, L("Workers.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}