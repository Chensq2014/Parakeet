using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 定义模块权限名
    /// </summary>
    public static class SectionPermissions
    {
        public const string GroupName = "Sections";//"区域/地块"

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(SectionPermissions));
        }

        public static class Sections
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
            public const string Worker = GroupName + ".Worker";
            public const string WorkerDetail = GroupName + ".WorkerDetail";
            public const string Report = GroupName + ".Report";
        }
    }
}