//using System.Collections.Generic;
//using Microsoft.Extensions.Options;
//using System.Threading.Tasks;
//using Common.Helpers;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using System.Linq;

//namespace Parakeet.Net.EntityFrameworkCore
//{
//    /// <summary>
//    /// 字符串解析器
//    /// </summary>
//    [Dependency(ReplaceServices = true)]
//    public class ConnectionStringResolver : DefaultConnectionStringResolver
//    {
//        //private readonly AreaTenantPool _areaTenantPool;
//        public ConnectionStringResolver(IOptionsMonitor<AbpDbConnectionOptions> options) : base(options)
//        {
//            //_areaTenantPool = areaTenantPool;
//        }


//        public override async Task<string> ResolveAsync(string connectionStringName = null)
//        {
//            //if (connectionStringName != null && Magics.STABLE_DB.Contains(connectionStringName))
//            //{
//            //    //从freesql缓存中读取 当前区域的 租户  再按年 获取租户的年数据库 连接字符串
//            //    var currentYear = DateTime.Now.Year;
//            //    var tenant = _areaTenantPool[connectionStringName];
//            //    var connectionString = tenant.TenantDbConnectionStrings.First(m => m.IsMaster && m.Year == currentYear).Value;
//            //    return connectionString;
//            //}
//            var connectionStringNames = new List<string>
//            {
//                "Default",
//                "MultiTenant",
//                "MySql",
//                "PgSql",
//                "SqlServer",
//                "Write",
//                "Read"
//            };
//            var connectionStringValue = await base.ResolveAsync(connectionStringName);
//            return connectionStringNames.Contains(connectionStringName)
//                ? EncodingEncryptHelper.DEncrypt(connectionStringValue)
//                : connectionStringValue;
//        }
//    }
//}
