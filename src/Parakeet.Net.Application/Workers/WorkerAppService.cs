﻿using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.WorkerTypes
{
    /// <summary>
    /// 工人服务
    /// </summary>
    public class WorkerAppService : BaseNetAppService<Worker>, IWorkerAppService
    {
        private readonly IPortalRepository<Worker> _workerRepository;
        public WorkerAppService(IPortalRepository<Worker> workerRepository) : base(workerRepository)
        {
            _workerRepository = workerRepository;
        }

        #region 重写父类 GridData

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<WorkerDto>(), loadOptions);
            return result;
        }

        #endregion

        /// <summary>
        /// 获取工人下拉列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<KeyValueDto<Guid, string>>> GetWorkerSelectList(InputIdNullDto input)
        {
            var projects = await (await GetAll())
                .WhereIf(input.Id.HasValue, x => x.Id == input.Id)
                .Select(m => new KeyValueDto<Guid, string>(m.Id, m.Name))
                .ToListAsync();
            return projects;
        }
    }
}