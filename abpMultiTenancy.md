# abp多租户调研  abpMultiTenancy
## 1、首先是多租户中间件  MultiTenancyMiddleware-->Invokeasync方法
   可以自定义一个相同代码的 CustomMultiTenancyMiddleware 中间件，调试看看整个过程  代码执行先后顺序

## 2、Invokeasync方法内部-->获取多租户方法 
       tenant = await _tenantConfigurationProvider.GetAsync(saveResolveResult: true);

## 3、依赖注入的 tenantConfigurationProvider源码

   protected virtual async Task<TenantConfiguration> FindTenantAsync(string tenantIdOrName)
    {
        if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
        {
            return await TenantStore.FindAsync(parsedTenantId);
        }
        else
        {
            return await TenantStore.FindAsync(tenantIdOrName);
        }
    }


所以 重写TenantStore 或者  tenantConfigurationProvider 都可以自定义多租户


## 4、继续MultiTenancyMiddleware中间件后续

通过注入的 _currentTenant.Change(tenant?.Id, tenant?.Name) 设置多租户

CurrentTenant一定不为空，因为它里面注入的ICurrentTenantAccessor

 public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

CurrentTenantAccessor实现类对象内部有一个BasicTenantInfo对象，默认为null
当有租户请求时，_currentTenant.Change(tenant?.Id, tenant?.Name) 方法内部会
设置BasicTenantInfo对象的值


所以有更改租户的业务逻辑 可以使用 注入接口 ICurrentTenant  使用 
_currentTenant.Change(tenant?.Id, tenant?.Name)  变更租户


## 5、注意多租户的使用方式  这些接口都是Scope范围….

### 重写 TenantResolver  可以调试看看  有哪些 ResolveContributor
	CurrentUserTenantResolveContributor: 如果当前用户已登录,从当前用户的声明中获取租户Id. 出于安全考虑,应该始终将其做为第一个Contributor.
	QueryStringTenantResolveContributor: 尝试从query string参数中获取当前租户,默认参数名为"__tenant".
	RouteTenantResolveContributor:尝试从当前路由中获取(URL路径),默认是变量名是"__tenant".所以,如果你的路由中定义了这个变量,就可以从路由中确定当前租户.
	HeaderTenantResolveContributor: 尝试从HTTP header中获取当前租户,默认的header名称是"__tenant".
	CookieTenantResolveContributor: 尝试从当前cookie中获取当前租户.默认的Cookie名称是"__tenant".


### 重写
MultiTenantConnectionStringResolver  调试 获取的连接字符串

-->ResolveAsync 里面 仍然用 TenantStore 来获取的。

# 注意 ：同一个线程里面调用多租户的问题   比如foreach循环里面，如果要循环读取多个租户数据库的服务，
           不能使用Lazy的方式去获取服务，需要使用框架提供的IServiceProvider去获取服务，因为abp的Lazy的方式即便获取的Itrainsent 服务，也都做了线程安全字典的缓存的。
           也就是说同一个线程内 瞬时生命周期的对象，使用lazy的方式是复用的。 单线程多租户的场景一定要使用IServiceProvider去获取服务，确保同一个线程内从容器中获取瞬时服务时都是新实例。


# 自定义注册(继承 ConventionalRegistrarBase)
AbpLazyServiceProvider  的 GetService 方法 (父类CachedServiceProviderBase的 GetService（从线程安全字典中获取）)

# AbpLazyServiceProvider 获取服务方法：

public virtual object LazyGetService(Type serviceType, Func<IServiceProvider, object> factory)
{
return CachedServices.GetOrAdd(
serviceType,
_ => new Lazy<object>(() => factory(ServiceProvider))
).Value;
}

# 从线程安全的缓存字典中获取  CachedServices 的服务
关系：AbpLazyServiceProvider : CachedServiceProviderBase, IAbpLazyServiceProvider, ITransientDependency
# CachedServiceProviderBase 类源码：
public abstract class CachedServiceProviderBase
{
protected IServiceProvider ServiceProvider { get; }
protected ConcurrentDictionary<Type, Lazy<object>> CachedServices { get; }

protected CachedServiceProviderBase(IServiceProvider serviceProvider)
{
ServiceProvider = serviceProvider;
CachedServices = new ConcurrentDictionary<Type, Lazy<object>>();
CachedServices.TryAdd(typeof(IServiceProvider), new Lazy<object>(() => ServiceProvider));
}

public virtual object GetService(Type serviceType)
{
return CachedServices.GetOrAdd(
serviceType,
_ => new Lazy<object>(() => ServiceProvider.GetService(serviceType))
).Value;
}
}


# 使用AbpLazyServiceProvider 时，一定要注意  同一个线程内，如果是瞬时生命周期的服务(ITransientDependency)，
实际上是获取的第一次添加到线程安全字典里面的服务，而不是每次都创建的。
在多租户场景下，
例如一个foreach循环里面 需要使用不同租户的服务时，
即使这个租户服务是瞬时生命周期的服务(ITransientDependency)，
当使用LazyServiceProvider的方式获取对象实例时，同一个线程内获取的是第一次进入foreach被缓存到 CachedServices字典的实例
这个时候，必须改为.NetCore 框架 原生的IServiceProvider去获取服务，保证同一个线程内，从容器获取的瞬时生命周期的对象随时都是重新实例化的对象。
