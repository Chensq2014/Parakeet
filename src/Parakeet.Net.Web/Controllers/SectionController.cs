using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Controllers;

namespace Parakeet.Net.Web.Controllers
{
    /// <summary>
    /// 小区区域管理
    /// </summary>
    //[ApiExplorerSettings(GroupName = "V2")]
    [Route("/api/parakeet/section/[action]")]
    public class SectionController : BaseNetEntityController<Section>
    {
        public SectionController(ISectionAppService baseService) : base(baseService)
        {
        }

        /// <summary>
        /// 列表 只提供列表查看
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// pivotGrid列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult ProductIndex()
        {
            return View();
        }

        /// <summary>
        /// Grid列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult WorkerIndex()
        {
            return View();
        }

        /// <summary>
        /// pivotGrid列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult WorkerDetailIndex()
        {
            return View();
        }

        /// <summary>
        /// 工种列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult Worker()
        {
            return View();
        }

        /// <summary>
        /// 工人列表
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult WorkerType()
        {
            return View();
        }
    }
}
