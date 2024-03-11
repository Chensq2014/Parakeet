using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;

namespace Parakeet.Net.WorkerTypes
{
    /// <summary>
    /// 工种服务
    /// </summary>
    public class WorkerTypeAppService : BaseNetAppService<WorkerType>, IWorkerTypeAppService
    {
        private readonly INetRepository<WorkerType> _workerTypeRepository;
        public WorkerTypeAppService(INetRepository<WorkerType> workerTypeRepository) : base(workerTypeRepository)
        {
            _workerTypeRepository = workerTypeRepository;
        }

        #region 重写父类 GridData

        /// <summary>
        /// 获取Get(扩展) 提供给devExtreme
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
        {
            var result = await DataSourceLoader.LoadAsync(await GridDto<WorkerTypeDto>(), loadOptions);
            return result;
        }


        #endregion
    }
}