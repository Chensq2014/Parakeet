using System;
using System.ComponentModel;

namespace Parakeet.Net.CustomAttributes
{
    /// <summary>
    /// Attribute抽象类，自定义的Attribute继承它
    /// 特性是在编译时确定的，构造函数/属性/字段，都不能用变量，
    /// 所以netMvc filter是不能注入的，在necore里面才可以注入
    /// </summary>
    //ALL：都可以使用特性修饰，Multiple：即使是同一个类可以重复修饰,Inherited：特性的子类是否可以继承
    //[AttributeUsage(AttributeTargets.All,AllowMultiple = true,Inherited = true)]
    [Description("属性公共基类")]
    public abstract class AbstractValidateAttribute : Attribute
    {
        ////抽象类可以有构造函数
        //protected AbstractValidateAtrribute() {}

        public abstract bool Validate<T>(T value);
    }
}
