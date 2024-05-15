//using AutoMapper.QueryableExtensions;
//using Common.Dtos;
//using Common.Entities;
//using Common.Interfaces;
//using DevExtreme.AspNet.Data;
//using DevExtreme.AspNet.Data.ResponseModel;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Parakeet.Net.Houses
//{
//    /// <summary>
//    /// 房间服务
//    /// </summary>
//    public class HouseAppService : BaseParakeetAppService<House>, IHouseAppService
//    {
//        private readonly IParakeetRepository<House> _houseRepository;
//        public HouseAppService(IParakeetRepository<House> houseRepository) : base(houseRepository)
//        {
//            _houseRepository = houseRepository;
//        }

//        #region 重写父类 GridData

//        /// <summary>
//        /// 获取Get(扩展) 提供给devExtreme masterGrid
//        /// </summary>
//        /// <param name="loadOptions"></param>
//        /// <returns></returns>
//        public override async Task<LoadResult> GetGet(DataSourceLoadOptionsBase loadOptions)
//        {
//            var id = GetRequestPrimarykey();
//            var query = (await Repository.GetQueryableAsync())
//                .AsNoTracking()
//                .Where(m => m.SectionId == id)
//                .OrderByDescending(o => o.Number)
//                .ProjectTo<HouseDto>(Configuration);
//            var result = await DataSourceLoader.LoadAsync(query, loadOptions);
//            return result;
//        }


//        #endregion
//    }
//}