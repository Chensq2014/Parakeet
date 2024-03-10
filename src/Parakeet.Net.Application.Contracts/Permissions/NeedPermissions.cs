using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 定义模块权限名
    /// </summary>
    public static class NeedPermissions
    {
        public const string GroupName = "Needs";

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(NeedPermissions));
        }

        public static class Needs
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}