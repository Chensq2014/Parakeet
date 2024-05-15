//using AutoMapper.QueryableExtensions;
//using Common.Dtos;
//using Common.Entities;
//using Common.Extensions;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Parakeet.Net.SectionWorkerDetails
//{
//    /// <summary>
//    /// 区域工人用工明细服务
//    /// </summary>
//    public class SectionWorkerDetailAppService : BaseParakeetAppService<SectionWorkerDetail>, ISectionWorkerDetailAppService
//    {
//        private readonly IParakeetRepository<SectionWorkerDetail> _sectionWorkerDetailRepository;
//        public SectionWorkerDetailAppService(IParakeetRepository<SectionWorkerDetail> sectionWorkerDetailRepository) : base(sectionWorkerDetailRepository)
//        {
//            _sectionWorkerDetailRepository = sectionWorkerDetailRepository;
//        }

//        #region 重写父类 GridData

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme masterGrid
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            //loadOptions.DefaultSort
//            var id = GetRequestPrimarykey();
//            var query = (await Repository.GetQueryableAsync()).AsNoTracking()
//                .Where(m => m.SectionWorkerId == id)
//                .ProjectTo<SectionWorkerDetailDto>(Configuration)
//                .OrderBy(o => o.CostTotal);
//            var result = await DataSourceLoader.LoadAsync(query, loadOptions);
//            return result;
//        }


//        #endregion

//        #region PivotGrid数据源

//        /// <summary>
//        /// 获取所有区域工人数据源
//        /// </summary> 
//        /// <param name="input"></param>
//        /// <returns></returns>
//        public async Task<List<SectionWorkerDetailDto>> PivotGrid([FromBody] SectionWorkerDetailPivotGridInputDto input)
//        {
//            var result = await (await GetAll())
//                .WhereIf(input.StartDate.HasValue, m => m.CreationTime >= input.StartDate)
//                .WhereIf(input.EndDate.HasValue, m => m.CreationTime <= input.EndDate)
//                .WhereIf(input.PositionName.HasValue(), m => m.PositionName == input.PositionName)
//                .WhereIf(input.SectionWorkerId.HasValue, m => m.SectionWorkerId == input.SectionWorkerId)
//                .ProjectTo<SectionWorkerDetailDto>(Configuration)
//                .ToListAsync();
//            return result;
//        }

//        #endregion
//    }
//}