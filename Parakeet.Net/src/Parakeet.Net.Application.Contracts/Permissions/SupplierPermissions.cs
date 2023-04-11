using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    public static class SupplierPermissions
    {
        public const string GroupName = "Suppliers";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(SupplierPermissions));
        }

        public static class Supplier
        {
            public const string Default = GroupName + ".Default";
            public const string Create = GroupName + ".Create";
            public const string Update = GroupName + ".Update";
            public const string Delete = GroupName + ".Delete";
        }
    }
}