//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using System.Threading.Tasks;

//namespace Parakeet.Net.WorkerTypes
//{
//    /// <summary>
//    /// 工人服务
//    /// </summary>
//    public class WorkerAppService : BaseParakeetAppService<Worker>, IWorkerAppService
//    {
//        private readonly IParakeetRepository<Worker> _workerRepository;
//        public WorkerAppService(IParakeetRepository<Worker> workerRepository) : base(workerRepository)
//        {
//            _workerRepository = workerRepository;
//        }

//        #region 重写父类 GridData

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<WorkerDto>(), loadOptions);
//            return result;
//        }
        
//        #endregion
//    }
//}