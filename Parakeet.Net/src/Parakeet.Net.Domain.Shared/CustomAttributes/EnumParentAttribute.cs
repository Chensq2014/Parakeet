using System;
using System.Reflection;

namespace Parakeet.Net.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumParentAttribute : AbstractValidateAttribute
    {
        public int Parent { get; set; }
        public EnumParentAttribute(int parent)
        {
            Parent = parent;
        }
        public override bool Validate<T>(T value)
        {
            //暂用Id属性
            var parent = value.GetType().GetField("Id").GetCustomAttribute<EnumParentAttribute>();
            return parent != null;
        }
    }
}
