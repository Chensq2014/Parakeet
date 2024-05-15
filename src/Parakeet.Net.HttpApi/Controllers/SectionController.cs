//using Common.Entities;
//using Common.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Parakeet.Net.Controllers
//{
//    /// <summary>
//    /// 小区区域管理
//    /// </summary>
//    //[ApiExplorerSettings(GroupName = "V2")]
//    [Route("/api/parakeet/section/[action]")]
//    public class SectionController : BaseParakeetEntityController<Section>
//    {
//        public SectionController(ISectionAppService baseService) : base(baseService)
//        {
//        }

//        /// <summary>
//        /// 列表 只提供列表查看
//        /// </summary>
//        /// <returns></returns>

//        [HttpGet]
//        public IActionResult Index()
//        {
//            return View();
//        }

//        /// <summary>
//        /// pivotGrid列表
//        /// </summary>
//        /// <returns></returns>

//        [HttpGet]
//        public IActionResult ProductIndex()
//        {
//            return View();
//        }

//    }
//}
