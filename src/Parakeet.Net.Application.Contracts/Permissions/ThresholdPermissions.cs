using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    public class ThresholdPermissions
    {
        public const string GroupName = "Thresholds";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(ThresholdPermissions));
        }

        public static class Threshold
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}
