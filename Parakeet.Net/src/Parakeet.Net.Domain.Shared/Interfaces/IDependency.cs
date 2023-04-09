using System.ComponentModel;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// 自定义接口 自动注册
    /// </summary>
    [Description("自定义依赖注入接口(自动注册)")]
    public interface IDependency
    {
        //接口中一般定义方法，特殊情况可使用（属性，索引器，事件）因为这些也都是不确定的
        //void Play();//方法可以
        //int Tag { get; set; }//属性可以
        //int this[int i] {  get; }//索引器可以
        //event Action DoNothing;//事件可以

        //stirng Remark = null;//字段不行
        //Delegate void NoAction();//委托不行
        //如果需要约束，一般选择接口，除非有代码需要重用就选择抽象类
        
    }
}
