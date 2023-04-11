using Parakeet.Net.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 数据模拟权限
    /// </summary>
    public class DeviceAnalogRulePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(DeviceAnalogRulePermissions.GroupName,L("DeviceAnalogRules"));

            myGroup.AddPermission(DeviceAnalogRulePermissions.DeviceAnalogRule.Default, L("DeviceAnalogRules.Default"));
            myGroup.AddPermission(DeviceAnalogRulePermissions.DeviceAnalogRule.Create, L("DeviceAnalogRules.Create"));
            myGroup.AddPermission(DeviceAnalogRulePermissions.DeviceAnalogRule.Update, L("DeviceAnalogRules.Update"));
            myGroup.AddPermission(DeviceAnalogRulePermissions.DeviceAnalogRule.Delete, L("DeviceAnalogRules.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<NetResource>(name);
        }
    }
}