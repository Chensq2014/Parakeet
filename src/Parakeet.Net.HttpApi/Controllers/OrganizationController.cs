using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 组织机构
    /// </summary>
    [Route("/api/parakeet/organization/[action]")]
    public class OrganizationController : BaseEntityController<Organization>
    {
        private readonly IOrganizationAppService _organizationService;
        public OrganizationController(IOrganizationAppService baseService) : base(baseService)
        {
            _organizationService = baseService;
        }

        [HttpGet, ResponseCache(Duration = 600)]//缓存600s
        public IActionResult Index()
        {
            return View();
        }
    }
}
