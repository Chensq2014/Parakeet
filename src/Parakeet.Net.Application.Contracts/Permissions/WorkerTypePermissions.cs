using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    public static class WorkerTypePermissions
    {
        public const string GroupName = "WorkerTypes";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(WorkerTypePermissions));
        }

        public static class WorkerType
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}