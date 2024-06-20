# abp多数据库支持
## 看下人家官方文档提供的：
CurrentUserTenantResolveContributor: 如果当前用户已登录,从当前用户的声明中获取租户Id. 出于安全考虑,应该始终将其做为第一个Contributor.
QueryStringTenantResolveContributor: 尝试从query string参数中获取当前租户,默认参数名为"__tenant".
RouteTenantResolveContributor:尝试从当前路由中获取(URL路径),默认是变量名是"__tenant".所以,如果你的路由中定义了这个变量,就可以从路由中确定当前租户.
HeaderTenantResolveContributor: 尝试从HTTP header中获取当前租户,默认的header名称是"__tenant".
CookieTenantResolveContributor: 尝试从当前cookie中获取当前租户.默认的Cookie名称是"__tenant".


## 调研结果：
看了源码，只需要把http请求中 QueryString 找到__tenantId改为projectId  
并且重写 QueryStringTenantResolveContributor 从主库中查找projectId 就解决了  
难点：重写 QueryStringTenantResolveContributor 时，需要访问主数据库 并从中得到ProjectId 转成租户Guid   podName等信息，
再从配置文件中结合以上信息，组装ConnectString ，然后abp框架使用从租户返回的connectString 初始化自己的dbcontext
每次请求进入都在使用自己租户(ProjectId)的数据库。


### 替换连接字符串解析器
ABP定义了 IConnectionStringResolver,并在需要连接字符串时使用它. 有两个预构建的实现:
    • DefaultConnectionStringResolver 根据上面"配置连接字符串"一节中定义的规则,使用 AbpDbConnectionOptions 选择连接字符串.
    • MultiTenantConnectionStringResolver 用于多租户应用程序,并尝试获取当前租户的已配置连接字符串(如果有). 它使用 ITenantStore 查找连接字符串. 它继承了 DefaultConnectionStringResolver, 如果没有为当前租户指定连接字符串则回退到基本逻辑.
如果需要自定义逻辑来确定连接字符串,可以实现 IConnectionStringResolver 接口(也可以从现有类派生)并使用依赖注入系统替换现有实现.


### 数据库：TenantDbContext
add-migration InitTenantDb -c TenantDbContext -o Migrations/TenantlMigrations
Script-Migration -From "[migration_pre文件名]" -To "[migration_next文件名]" -context TenantDbContext
update-database -context TenantDbContext

### 数据库：ProjectDbContext
add-migration InitProjectDb -c ProjectDbContext -o Migrations/ProjectMigrations
Script-Migration -From "[migration_pre文件名]" -To "[migration_next文件名]" -context ProjectDbContext
update-database -context ProjectDbContext