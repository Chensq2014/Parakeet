using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Entities;
using Parakeet.Net.Permissions;
using Parakeet.Net.Suppliers;

namespace Parakeet.Net.Controllers
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
