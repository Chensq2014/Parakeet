using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions;

public static class NetPermissions
{
    public const string GroupName = "Net";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NetPermissions));
    }

    public static class Net
    {
        public const string Default = GroupName;
        public const string Update = Default + ".Update";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
