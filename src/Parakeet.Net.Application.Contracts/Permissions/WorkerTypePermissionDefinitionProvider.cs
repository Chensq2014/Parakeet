using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 工种权限
    /// </summary>
    public class WorkerTypePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(WorkerTypePermissions.GroupName,L("WorkerTypes"));

            myGroup.AddPermission(WorkerTypePermissions.WorkerType.Default, L("WorkerTypes.Default"));
            myGroup.AddPermission(WorkerTypePermissions.WorkerType.Create, L("WorkerTypes.Create"));
            myGroup.AddPermission(WorkerTypePermissions.WorkerType.Update, L("WorkerTypes.Update"));
            myGroup.AddPermission(WorkerTypePermissions.WorkerType.Delete, L("WorkerTypes.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}