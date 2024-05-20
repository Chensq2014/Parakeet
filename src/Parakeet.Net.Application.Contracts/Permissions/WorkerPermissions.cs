using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    public static class WorkerPermissions
    {
        public const string GroupName = "Workers";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(WorkerPermissions));
        }

        public static class Worker
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}