using Common.Dtos;
using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Controllers;
using System.Threading.Tasks;

namespace Parakeet.Net.Web.Controllers
{
    /// <summary>
    /// 区域租户管理
    /// </summary>
    [Route("/api/parakeet/areaTenant/[action]")]
    public class AreaTenantController : BaseNetEntityController<AreaTenant>
    {

        public AreaTenantController(IAreaTenantAppService baseService) : base(baseService)
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


        [HttpGet]
        public async Task<IActionResult> TenantConnectStringIndex(InputIdDto input)
        {
            var entity = await BaseService.GetByPrimaryKey(input);
            var dto = ObjectMapper.Map<AreaTenant, AreaTenantListDto>(entity);
            return View(dto);
        }

        [HttpGet]
        public IActionResult AreaTreeListIndex()
        {
            return View();
        }

    }
}
