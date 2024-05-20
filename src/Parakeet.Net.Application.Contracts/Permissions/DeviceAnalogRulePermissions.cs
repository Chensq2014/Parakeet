using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    public static class DeviceAnalogRulePermissions
    {
        public const string GroupName = "DeviceAnalogRules";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(DeviceAnalogRulePermissions));
        }

        public static class DeviceAnalogRule
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}