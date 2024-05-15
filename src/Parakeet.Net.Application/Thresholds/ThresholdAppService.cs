//using AutoMapper.QueryableExtensions;
//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;

//namespace Parakeet.Net.Thresholds
//{
//    /// <summary>
//    /// 阈值管理
//    /// </summary>
//    public class ThresholdAppService : BaseParakeetAppService<Threshold>, IThresholdAppService
//    {
//        public ThresholdAppService(IParakeetRepository<Threshold> baseRepository) : base(baseRepository)
//        {
//        }


//        #region 重写GetGridData 获取分页数据GetPagedResult

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<ThresholdDto>(), loadOptions);
//            return result;
//        }

//        /// <summary>
//        /// 获取GridData(Post扩展)  提供给devExtreme
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GridData(LoadOptionInputDto input)
//        {
//            //根据input过滤数据源
//            var result = await DataSourceLoader.LoadAsync((await GridDto<ThresholdDto>()).Where(m => m.Id == input.Id), input.LoadOptions);
//            return result;
//        }

//        ///// <summary>
//        /////  获取列表数据(分页)
//        ///// </summary>
//        ///// <param name="input"></param>
//        ///// <returns></returns>
//        //public async Task<IPagedResult<ThresholdDto>> GetPagedResult(GetThresholdsInputDto input)
//        //{
//        //    //return await base.GetPagedResult<ThresholdDto>(input);
//        //    return await AbpQueryableExtensions.WhereIf(GetBaseRepository()
//        //            .AsNoTracking(), input.Filter.HasValue(), x =>
//        //            x.Name.Contains(input.Filter))
//        //        .OrderBy(input.Sorting)
//        //        .ProjectTo<ThresholdDto>(Configuration)
//        //        .ToPageResultAsync(input, input.FindTotalCount);
//        //}

//        /// <summary>
//        ///  获取所有供应商列表数据
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<ThresholdDto>> GetThresholdFilterList(InputIdNullDto input)
//        {
//            return await (await Repository.GetQueryableAsync())
//                .AsNoTracking()
//                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
//                .ProjectTo<ThresholdDto>(Configuration)
//                .ToListAsync();
//        }


//        /// <summary>
//        /// 获取供应商下拉列表
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<SelectBox>> GetThresholdSelectBox(InputIdNullDto input)
//        {
//            var items = await (await Repository.GetQueryableAsync()).AsNoTracking()
//                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
//                .Select(m => new SelectBox(m.Name,m.Id))
//                .ToListAsync();
//            return items;
//        }

//        #endregion
//    }
//}
