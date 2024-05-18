//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using System.Threading.Tasks;

//namespace Parakeet.Net.Sections
//{
//    /// <summary>
//    /// 小区区域服务
//    /// </summary>
//    public class SectionAppService : BaseNetAppService<Section>, ISectionAppService
//    {
//        private readonly INetRepository<Section> _sectionRepository;
//        public SectionAppService(INetRepository<Section> sectionRepository) : base(sectionRepository)
//        {
//            _sectionRepository = sectionRepository;
//        }

//        #region 重写父类 GridData

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var result = await DataSourceLoader.LoadAsync(await GridDto<SectionDto>(), loadOptions);
//            return result;
//        }


//        #endregion
//    }
//}