using Parakeet.Net.Locks;
using System.Reflection;
using System.Threading.Tasks;
using Common.CustomAttributes;
using Common.Locks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Users;

namespace Parakeet.Net.Aop
{
    /// <summary>
    /// 允许依赖注入的缓存锁拦截器 (继承自IAbpInterceptor)
    /// 拦截器的注册工具类AuditingInterceptorRegistrar(静态类)被CLR自动调用时,拦截器容器中的这些拦截器被实例化?待验证
    /// </summary>
    public class CacheLockInterceptor : AbpInterceptor, ITransientDependency
    {
        /// <summary>
        /// 当前登录人 abp框架IdentityServer自动获取
        /// </summary>
        public ICurrentUser CurrentUser { get; set; }

        /// <summary>
        /// 方法上的用户自定义锁定属性,使用此属性的方法会进行lock
        /// </summary>
        public UserCacheLockAttribute LockAttribute { get; set; }

        /// <summary>
        /// 自定义业务锁
        /// </summary>
        private readonly ILock _lock;

        /// <summary>
        /// 拦截器构造函数依赖注入自定义锁
        /// </summary>
        /// <param name="lock"></param>
        public CacheLockInterceptor(ILock @lock)
        {
            _lock = @lock;
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            LockAttribute = invocation.Method.GetCustomAttribute<UserCacheLockAttribute>();
            var operationName = LockAttribute?.OperationName ?? invocation.Method.Name;
            using (new UserLock(_lock, CurrentUser?.Id, operationName, LockAttribute?.Expiration).CheckLock())
            {
                await invocation.ProceedAsync();
            }
        }
    }
}
