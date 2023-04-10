using System.Threading.Tasks;

namespace Parakeet.Net.Extensions
{
    ///// <summary>
    ///// 扩展方法：静态类里面的静态方法，第一个参数类型前面加上this
    ///// 扩展方法调用，很像实例方法，就像扩展了类的逻辑
    ///// 1、产生原因：第三方的类，不适合修改源码，可以通过扩展方法增加逻辑
    ///// 2、调用：优先调用实例方法(最怕增加了扩展方法，类中同时又增加了相同名称的方法)
    ///// 3、适用场景：netcore里面 组件式开发的扩展 定义接口或者类，按照最小需求，但是在开发时经常需要加一些方法，可以通过扩展方法
    /////    例如：context.Response.WriteAsync   中间件的注册 use map when...等等都是扩展方法
    ///// 4、扩展一些常见的操作
    ///// </summary>
    //public static class TaskExtensions
    //{
    //    private static readonly Task _defaultCompleted = Task.FromResult<AsyncVoid>(default(AsyncVoid));
    //    private struct AsyncVoid
    //    {
    //    }
    //    public static Task FromResult(this Task task)
    //    {
    //        return _defaultCompleted;
    //    }
    //}
}
