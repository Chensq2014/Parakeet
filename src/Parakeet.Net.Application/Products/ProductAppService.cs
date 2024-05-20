using AutoMapper.QueryableExtensions;
using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Products
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductAppService : BaseNetAppService<Product>, IProductAppService
    {
        private readonly IPortalRepository<Product> _productRepository;
        public ProductAppService(IPortalRepository<Product> productRepository) : base(productRepository)
        {
            _productRepository = productRepository;
        }

        #region 重写父类 GridData

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme masterGrid
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            //loadOptions.DefaultSort
            var id = GetRequestPrimarykey();
            var query = (await Repository.GetQueryableAsync()).AsNoTracking()
                .Where(m => m.HouseId == id)
                .ProjectTo<ProductDto>(Configuration)
                .OrderBy(o => o.ChargeType);
            var result = await DataSourceLoader.LoadAsync(query, loadOptions);
            return result;
        }


        #endregion

        #region PivotGrid数据源
        /// <summary>
        /// 获取所有产品数据源 默认只查看半个月内的数据
        /// </summary> 
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<ProductListDto>> PivotGrid([FromBody] ProductPivotGridInputDto input)
        {
            var result = await (await GetAll()).AsNoTracking()
                .WhereIf(input.StartDate.HasValue, m => m.CreationTime >= input.StartDate)
                .WhereIf(input.EndDate.HasValue, m => m.CreationTime <= input.EndDate)
                .WhereIf(input.ChargeType.HasValue, m => m.ChargeType == input.ChargeType)
                .ProjectTo<ProductListDto>(Configuration)
                .ToListAsync();
            return result;
        }

        #endregion
    }
}