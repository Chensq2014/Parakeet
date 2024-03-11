using AutoMapper.QueryableExtensions;
using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Parakeet.Net.LinqExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Common.Extensions;
using Volo.Abp.Application.Dtos;

namespace Parakeet.Net.Suppliers
{
    /// <summary>
    /// 供应商服务
    /// </summary>
    public class SupplierAppService : BaseNetAppService<Supplier>, ISupplierAppService
    {
        public SupplierAppService(INetRepository<Supplier> baseRepository) : base(baseRepository)
        {
        }


        #region 重写GetGridData 获取分页数据GetPagedResult
        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<SupplierDto>(), loadOptions);
            return result;
        }

        /// <summary>
        /// 获取GridData(Post扩展)  提供给devExtreme
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GridData(LoadOptionInputDto input)
        {
            //根据input过滤数据源
            var result = await DataSourceLoader.LoadAsync((await GridDto<SupplierDto>()).Where(m => m.Id == input.Id), input.LoadOptions);
            return result;
        }

        /// <summary>
        ///  获取列表数据(分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IPagedResult<SupplierDto>> GetPagedResult(GetSuppliersInputDto input)
        {
            //return await base.GetPagedResult<SupplierDto>(input);
            return await (await Repository.GetQueryableAsync())
                .AsNoTracking()
                .WhereIf(input.Filter.HasValue(), x =>
                    x.Name.Contains(input.Filter))
                .OrderBy(input.Sorting)
                .ProjectTo<SupplierDto>(Configuration)
                .ToPageResultAsync(input, input.FindTotalCount);
        }

        /// <summary>
        ///  获取所有供应商列表数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<SupplierDto>> GetSupplierFilterList(InputIdNullDto input)
        {
            return await (await Repository.GetQueryableAsync())
                .AsNoTracking()
                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
                .ProjectTo<SupplierDto>(Configuration)
                .ToListAsync();
        }


        /// <summary>
        /// 获取供应商下拉列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<KeyValueDto<Guid, string>>> GetSupplierSelectList(InputIdNullDto input)
        {
            var projects = await (await Repository.GetQueryableAsync()).AsNoTracking()
                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
                .Select(m => new KeyValueDto<Guid, string>(m.Id, m.Name))
                .ToListAsync();
            return projects;
        }

        #endregion
    }
}
