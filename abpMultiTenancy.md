1、首先是多租户中间件  MultiTenancyMiddleware-->Invokeasync方法
   可以自定义一个相同代码的 CustomMultiTenancyMiddleware 中间件，调试看看整个过程  代码执行先后顺序

2、Invokeasync方法内部-->获取多租户方法 
       tenant = await _tenantConfigurationProvider.GetAsync(saveResolveResult: true);

3、依赖注入的 tenantConfigurationProvider源码

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


4、继续MultiTenancyMiddleware中间件后续

通过注入的 _currentTenant.Change(tenant?.Id, tenant?.Name) 设置多租户

CurrentTenant一定不为空，因为它里面注入的ICurrentTenantAccessor

 public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

CurrentTenantAccessor实现类对象内部有一个BasicTenantInfo对象，默认为null
当有租户请求时，_currentTenant.Change(tenant?.Id, tenant?.Name) 方法内部会
设置BasicTenantInfo对象的值


所以有更改租户的业务逻辑 可以使用 注入接口 ICurrentTenant  使用 
_currentTenant.Change(tenant?.Id, tenant?.Name)  变更租户


5、注意多租户的使用方式  这些接口都是Scope范围….

重写 TenantResolver  可以调试看看  有哪些 ResolveContributor
	CurrentUserTenantResolveContributor: 如果当前用户已登录,从当前用户的声明中获取租户Id. 出于安全考虑,应该始终将其做为第一个Contributor.
	QueryStringTenantResolveContributor: 尝试从query string参数中获取当前租户,默认参数名为"__tenant".
	RouteTenantResolveContributor:尝试从当前路由中获取(URL路径),默认是变量名是"__tenant".所以,如果你的路由中定义了这个变量,就可以从路由中确定当前租户.
	HeaderTenantResolveContributor: 尝试从HTTP header中获取当前租户,默认的header名称是"__tenant".
	CookieTenantResolveContributor: 尝试从当前cookie中获取当前租户.默认的Cookie名称是"__tenant".


重写
MultiTenantConnectionStringResolver  调试 获取的连接字符串

-->ResolveAsync 里面 仍然用 TenantStore 来获取的。


