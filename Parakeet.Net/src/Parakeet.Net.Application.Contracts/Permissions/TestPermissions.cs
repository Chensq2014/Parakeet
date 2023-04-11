using Volo.Abp.Reflection;

namespace Parakeet.Net.Permissions
{
    /// <summary>
    /// 定义模块权限名
    /// </summary>
    public static class TestPermissions
    {
        public const string GroupName = "Tests";

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(TestPermissions));
        }

        public static class Tests
        {
            public const string Default = GroupName + ".Test";
            public const string RabbitMQ = Default + ".RabbitMQ";
            public const string AddProducer = Default + ".RabbitMQ.AddProducer";
        }
    }
}