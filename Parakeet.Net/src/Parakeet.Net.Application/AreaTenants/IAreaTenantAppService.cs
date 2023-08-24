using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Parakeet.Net.Entities;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Parakeet.Net.AreaTenants
{
    /// <summary>
    /// 区域租户及租户连接字符串管理
    /// </summary>
    public interface IAreaTenantAppService:IBaseNetAppService<AreaTenant>, ITransientDependency
    {
        #region 连接字符串分页管理

        /// <summary>
        /// 获取租户所有区域连接字符串分页集合
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        Task<LoadResult> GetTenantDbConnectionString(DataSourceLoadOptionsBase loadOptions);

        /// <summary>
        /// 添加连接串
        /// </summary>
        /// <returns></returns>
        [Description("添加连接串")]
        Task<Guid> InsertTenantDbConnectionString();

        /// <summary>
        /// Update修改连接串
        /// </summary>
        /// <returns></returns>
        [Description("修改连接串")]
        Task UpdateTenantDbConnectionString();

        /// <summary>
        /// 根据主键Id删除连接串
        /// </summary>
        /// <returns></returns>
        [Description("删除连接串")]
        Task DeleteTenantDbConnectionString();

        #endregion
    }
}
