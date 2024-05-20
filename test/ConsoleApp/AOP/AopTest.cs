using Serilog;
using System;
using System.Reflection;

namespace ConsoleApp.AOP
{
    /// <summary>
    ///逻辑还原：动态代理
    ///1、有一个基础的接口定义规范
    ///2、想办法让用户传一个啥进来，然后，不改变原来函数的动作的情况下，我给这个函数增加一个啥功能
    ///3、可能得有注册，因为我也不知道代理啥东西
    ///
    /// 所谓的函数调用是什么？？？？
    /// Invoke--所谓的Invoke不过是windows的一个message被当前程序截获被指派给这个函数使用
    /// 肯定要用到反射 DispatchProxy
    /// </summary>
    public class AopTest : DispatchProxy
    {
        /// <summary>
        /// 这个函数应该是能返回一个动态劫持后的那个对象 AopTest
        /// </summary>
        public AopTest Register(Type targetType)
        {
            var methodInfo = typeof(DispatchProxy).GetMethod(nameof(Create));
            methodInfo = methodInfo?.MakeGenericMethod(targetType, typeof(AopTest));
            var result =  (AopTest)methodInfo?.Invoke(null, null);
            //var result = (AopTest)Activator.CreateInstance(typeof(AopTest));
            return result;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Log.Logger.Debug($"执行前");
            Log.Logger.Debug($"{targetMethod.Name}.......");
            var result = "";//targetMethod.Invoke(null,args);
            Log.Logger.Debug($"执行后");
            return result;
        }
    }
}
