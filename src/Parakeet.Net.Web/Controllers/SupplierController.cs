using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Controllers;
using Parakeet.Net.Permissions;

namespace Parakeet.Net.Web.Controllers
{
    [Authorize(SupplierPermissions.Supplier.Default)]
    public class SupplierController : BaseEntityController<Supplier>
    {
        private readonly IAuthorizationService _authorizationService;
        public SupplierController(ISupplierAppService baseService, IAuthorizationService authorizationService) : base(baseService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Route("Supplier-list")]
        [Authorize(SupplierPermissions.Supplier.Default)]
        public IActionResult Index()
        {
            ViewBag.AuthorizationService = _authorizationService;
            return View();
        }
    }
}
