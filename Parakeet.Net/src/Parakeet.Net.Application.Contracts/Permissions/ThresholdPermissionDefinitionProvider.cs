using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 供应商权限
    /// </summary>
    public class ThresholdPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(ThresholdPermissions.GroupName,L("Thresholds"));

            myGroup.AddPermission(ThresholdPermissions.Threshold.Default, L("Thresholds.Default"));
            myGroup.AddPermission(ThresholdPermissions.Threshold.Create, L("Thresholds.Create"));
            myGroup.AddPermission(ThresholdPermissions.Threshold.Update, L("Thresholds.Update"));
            myGroup.AddPermission(ThresholdPermissions.Threshold.Delete, L("Thresholds.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}