using Common.CustomAttributes;
using System;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.Aop
{
    /// <summary>
    /// 添加到模块的预加载方法中注册 
    /// </summary>
    public class CacheLockInterceptorRegistrar
    {
        /// <summary>
        /// 把拦截器注入到
        /// </summary>
        /// <param name="context"></param>
        public static void RegisterIfNeed(IOnServiceRegistredContext context)
        {
            if (ShouldIntercept(context.ImplementationType))
            {
                context.Interceptors.TryAdd<CacheLockInterceptor>();
            }
        }

        private static bool ShouldIntercept(Type type)
        {
            // 判断类型是否使用了 UserCacheLock 特性，使用了则应用审计日志拦截器。
            //Console.WriteLine(type.FullName);
            if (type.IsDefined(typeof(UserCacheLockAttribute), true))
            {
                return true;
            }

            return false;
        }
    }
}
