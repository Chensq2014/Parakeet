using Serilog;

namespace ConsoleApp
{
    /// <summary>
    /// 单例类：适用于一个构造对象很耗时耗资源类型
    /// 饿汉式 只要使用类就已经被构造，以后再不用再构造了
    /// </summary>
    public class SingletonTest
    {
        /// <summary>
        /// 静态字段：在第一次使用这个类之前，由CLR保证，初始化且只初始化一次
        /// 这个比构造函数还早
        /// </summary>
        private static SingletonTest _singleton = new SingletonTest();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        public static SingletonTest Instance()
        {
            Log.Logger.Debug($"{nameof(SingletonTest)}被构造一次");
            return _singleton;//饿汉式  只要使用类就已经被构造 避免了多线程创建多个实例的bug
        }
    }
}
