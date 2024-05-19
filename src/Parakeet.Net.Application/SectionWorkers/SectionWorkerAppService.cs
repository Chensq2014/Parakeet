using AutoMapper.QueryableExtensions;
using Common.Dtos;
using Common.Entities;
using Common.Extensions;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.SectionWorkers
{
    /// <summary>
    /// 区域工人服务
    /// </summary>
    public class SectionWorkerAppService : BaseNetAppService<SectionWorker>, ISectionWorkerAppService
    {
        private readonly IPortalRepository<SectionWorker> _sectionWorkerRepository;
        public SectionWorkerAppService(IPortalRepository<SectionWorker> sectionWorkerRepository) : base(sectionWorkerRepository)
        {
            _sectionWorkerRepository = sectionWorkerRepository;
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
                .Where(m => m.SectionId == id)
                .ProjectTo<SectionWorkerDto>(Configuration)
                .OrderBy(o => o.CostTotal);
            var result = await DataSourceLoader.LoadAsync(query, loadOptions);
            return result;
        }


        #endregion

        #region PivotGrid数据源
        /// <summary>
        /// 获取所有区域工人数据源 默认只查看半个月内的数据
        /// </summary> 
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<SectionWorkerDto>> PivotGrid([FromBody] SectionWorkerPivotGridInputDto input)
        {
            var result = await (await GetAll())
                .WhereIf(input.StartDate.HasValue, m => m.CreationTime >= input.StartDate)
                .WhereIf(input.EndDate.HasValue, m => m.CreationTime <= input.EndDate)
                .WhereIf(input.WorkerId.HasValue, m => m.WorkerId == input.WorkerId)
                .WhereIf(input.WorkerTypeId.HasValue, m => m.WorkerTypeId == input.WorkerTypeId)
                .WhereIf(input.Name.HasValue(), m => m.Name == input.Name)
                .ProjectTo<SectionWorkerDto>(Configuration)
                .ToListAsync();
            return result;
        }

        #endregion
    }
}