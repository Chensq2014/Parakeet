//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Common;
//using Volo.Abp;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Common.Helpers;

//namespace Parakeet.Net.MultiTenancy
//{
//    /// <summary>
//    /// 替换默认实现 IConnectionStringResolver, ITransientDependency
//    /// EncodingEncryptHelper.DEncrypt 确保多租户数据库里跟配置文件里一样存储的是连接字符串的加密字符串
//    /// </summary>
//    [Dependency(ReplaceServices = true)]
//    public class TenantConnectionStringResolver : DefaultConnectionStringResolver//MultiTenantConnectionStringResolver// 
//    {
//        private readonly ICurrentTenant _currentTenant;

//        private readonly IServiceProvider _serviceProvider;

//        public TenantConnectionStringResolver(IOptionsMonitor<AbpDbConnectionOptions> options, ICurrentTenant currentTenant, IServiceProvider serviceProvider)
//            : base(options)
//        {
//            _currentTenant = currentTenant;
//            _serviceProvider = serviceProvider;
//        }

//        public override async Task<string> ResolveAsync(string? connectionStringName = null)
//        {
//            if (!_currentTenant.Id.HasValue)
//            {
//                if (connectionStringName == null)
//                {
//                    return Options.ConnectionStrings.Default;
//                }

//                string connectionStringOrNull = Options.GetConnectionStringOrNull(connectionStringName);
//                if (!connectionStringOrNull.IsNullOrEmpty())
//                {
//                    return connectionStringOrNull;
//                }

//                return null;
//                //var conn= await base.ResolveAsync(connectionStringName).ConfigureAwait(continueOnCapturedContext: false);
//                //return conn;
//            }

//            TenantConfiguration tenantConfiguration = await FindTenantConfigurationAsync(_currentTenant.Id.Value).ConfigureAwait(continueOnCapturedContext: false);
//            if (tenantConfiguration == null || tenantConfiguration.ConnectionStrings.IsNullOrEmpty())
//            {
//                return await base.ResolveAsync(connectionStringName).ConfigureAwait(continueOnCapturedContext: false);
//            }

//            string text = tenantConfiguration.ConnectionStrings?.Default;
//            if (connectionStringName == null || connectionStringName == ConnectionStrings.DefaultConnectionStringName)
//            {
//                return (!text.IsNullOrWhiteSpace()) ? text : base.Options.ConnectionStrings.Default;
//            }

//            string text2 = tenantConfiguration.ConnectionStrings?.GetOrDefault(connectionStringName);
//            if (!text2.IsNullOrWhiteSpace())
//            {
//                return text2;
//            }

//            AbpDatabaseInfo mappedDatabaseOrNull = base.Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
//            if (mappedDatabaseOrNull != null && mappedDatabaseOrNull.IsUsedByTenants)
//            {
//                text2 = tenantConfiguration.ConnectionStrings?.GetOrDefault(mappedDatabaseOrNull.DatabaseName);
//                if (!text2.IsNullOrWhiteSpace())
//                {
//                    return text2;
//                }
//            }

//            if (!text.IsNullOrWhiteSpace())
//            {
//                return text;
//            }

//            return await base.ResolveAsync(connectionStringName).ConfigureAwait(continueOnCapturedContext: false);
//        }

//        [Obsolete("Use ResolveAsync method.")]
//        public override string Resolve(string? connectionStringName = null)
//        {
//            if (!_currentTenant.Id.HasValue)
//            {
//                return base.Resolve(connectionStringName);
//            }

//            TenantConfiguration tenantConfiguration = FindTenantConfiguration(_currentTenant.Id.Value);
//            if (tenantConfiguration == null || tenantConfiguration.ConnectionStrings.IsNullOrEmpty())
//            {
//                return base.Resolve(connectionStringName);
//            }

//            string text = tenantConfiguration.ConnectionStrings?.Default;
//            if (connectionStringName == null || connectionStringName == ConnectionStrings.DefaultConnectionStringName)
//            {
//                if (text.IsNullOrWhiteSpace())
//                {
//                    return base.Options.ConnectionStrings.Default;
//                }

//                return text;
//            }

//            string text2 = tenantConfiguration.ConnectionStrings?.GetOrDefault(connectionStringName);
//            if (!text2.IsNullOrWhiteSpace())
//            {
//                return text2;
//            }

//            if (!text.IsNullOrWhiteSpace())
//            {
//                return text;
//            }

//            string orDefault = base.Options.ConnectionStrings.GetOrDefault(connectionStringName);
//            if (!orDefault.IsNullOrWhiteSpace())
//            {
//                return orDefault;
//            }

//            string @default = base.Options.ConnectionStrings.Default;
//            if (!@default.IsNullOrWhiteSpace())
//            {
//                return @default;
//            }

//            throw new AbpException("No connection string defined!");
//        }

//        protected virtual async Task<TenantConfiguration?> FindTenantConfigurationAsync(Guid tenantId)
//        {
//            using IServiceScope serviceScope = _serviceProvider.CreateScope();
//            return await serviceScope.ServiceProvider.GetRequiredService<ITenantStore>().FindAsync(tenantId).ConfigureAwait(continueOnCapturedContext: false);
//        }

//        [Obsolete("Use FindTenantConfigurationAsync method.")]
//        protected virtual TenantConfiguration? FindTenantConfiguration(Guid tenantId)
//        {
//            using IServiceScope serviceScope = _serviceProvider.CreateScope();
//            return serviceScope.ServiceProvider.GetRequiredService<ITenantStore>().Find(tenantId);
//        }

//    }
//}
