using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.EntityFrameworkCore
{
    /// <summary>
    /// 字符串解析器
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class ConnectionStringResolver : DefaultConnectionStringResolver
    {
        //private readonly AreaTenantPool _areaTenantPool;
        public ConnectionStringResolver(IOptionsMonitor<AbpDbConnectionOptions> options) : base(options)
        {
            //_areaTenantPool = areaTenantPool;
        }


        public override async Task<string> ResolveAsync(string connectionStringName = null)
        {
            //if (connectionStringName != null && Magics.STABLE_DB.Contains(connectionStringName))
            //{
            //    //从freesql缓存中读取 当前区域的 租户  再按年 获取租户的年数据库 连接字符串
            //    var currentYear = DateTime.Now.Year;
            //    var tenant = _areaTenantPool[connectionStringName];
            //    var connectionString = tenant.TenantDbConnectionStrings.First(m => m.IsMaster && m.Year == currentYear).Value;
            //    return connectionString;
            //}
            return await base.ResolveAsync(connectionStringName);
        }
    }
}
