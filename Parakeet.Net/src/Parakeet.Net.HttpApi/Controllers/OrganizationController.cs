using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Entities;
using Parakeet.Net.Organizations;

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
