using JetBrains.Annotations;
using Parakeet.Net.Entities;
using System;
using System.Linq;

namespace Parakeet.Net.Extensions
{
    /// <summary>
    /// 关于类型的扩展
    /// </summary>
    public static class TypeExtensions
    {
        //netcore框架做了优化 只有在静态类使用时，构造函数才会被执行了
        static TypeExtensions()
        {
            //测试
            var baseType = typeof(BaseEntity<>);//FullAuditedAggregateRoot
            var children = baseType.Assembly.GetTypes()
                .Where(x => x.BaseType?.IsGenericType == true && x.IsImplementedBaseType(baseType))
                .ToList();
        }

        /// <summary>
        /// 是否某个父(泛型)类型的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static bool IsImplementedBaseType([NotNull] this Type type, [NotNull] Type generic)
        {
            while (type != null && type != typeof(object))
            {
                var isMatch = IsCurrentGenericType(type);
                if (isMatch)
                {
                    return true;
                }
                type = type.BaseType;
            }
            bool IsCurrentGenericType(Type t) => generic == (t.IsGenericType ? t.GetGenericTypeDefinition() : t);
            return false;
        }
    }
}
