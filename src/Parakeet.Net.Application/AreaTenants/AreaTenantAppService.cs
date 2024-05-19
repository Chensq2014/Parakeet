using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Parakeet.Net.AreaTenants
{
    /// <summary>
    /// 区域租户及租户连接字符串管理
    /// </summary>
    public class AreaTenantAppService : BaseNetAppService<AreaTenant>, IAreaTenantAppService
    {
        private readonly IPortalRepository<AreaTenant> _areaTenantRepository;
        private readonly IRepository<AreaTenantDbConnectionString, Guid> _tenantDbConnectionString;

        public AreaTenantAppService(IPortalRepository<AreaTenant> areaTenantRepository,
            IRepository<AreaTenantDbConnectionString, Guid> tenantDbConnectionString) : base(areaTenantRepository)
        {
            _areaTenantRepository = areaTenantRepository;
            _tenantDbConnectionString = tenantDbConnectionString;
        }

        #region 区域租户管理

        #region 重写父类 GridData

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<AreaTenantListDto>(), loadOptions);
            return result;
        }

        //public override async Task UpdateUpdate()
        //{
        //    var entity = await GetByPrimaryKey(new InputIdDto { Id = GetTPrimaryKey() });
        //    Newtonsoft.Json.JsonConvert.PopulateObject(GetValuesString(), entity);
        //    entity.LastModificationTime = DateTime.Now;
        //}

        #endregion


        #region 连接字符串分页管理

        /// <summary>
        /// 获取租户所有区域连接字符串分页集合
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public async Task<LoadResult> GetTenantDbConnectionString(DataSourceLoadOptionsBase loadOptions)
        {
            var id = GetRequestPrimarykey();
            var query = (await _tenantDbConnectionString.GetQueryableAsync()).AsNoTracking()
                .Where(m => m.AreaTenantId == id)
                .OrderByDescending(o => o.Date)
                .ProjectTo<TenantDbConnectionStringDto>(Configuration);
            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }


        /// <summary>
        /// 添加连接串
        /// </summary>
        /// <returns></returns>
        [Description("添加连接串")]
        public async Task<Guid> InsertTenantDbConnectionString()
        {
            var formData = ContextAccessor.HttpContext?.Request.Form["values"];
            var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<AreaTenantDbConnectionString>(formData);
            await _tenantDbConnectionString.InsertAsync(entity);
            return entity.Id;
        }

        /// <summary>
        /// Update修改连接串
        /// </summary>
        /// <returns></returns>
        [Description("修改连接串")]
        public async Task UpdateTenantDbConnectionString()
        {
            var key = GetRequestPrimarykey();
            if (key.HasValue)
            {
                var entity = await _tenantDbConnectionString.GetAsync(key.Value);
                Newtonsoft.Json.JsonConvert.PopulateObject(GetFormValuesString(), entity);
            }
        }

        /// <summary>
        /// 根据主键Id删除连接串
        /// </summary>
        /// <returns></returns>
        [Description("删除连接串")]
        public async Task DeleteTenantDbConnectionString()
        {
            await _tenantDbConnectionString.DeleteAsync(m => m.Id == GetRequestPrimarykey());
        }

        #endregion

        #endregion

    }
}
