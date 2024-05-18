//using Common.Entities;
//using Common.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Parakeet.Net.Permissions;

//namespace Parakeet.Net.Controllers
//{
//    [Authorize(ThresholdPermissions.Threshold.Default)]
//    public class ThresholdController : BaseNetEntityController<Threshold>
//    {
//        private readonly IAuthorizationService _authorizationService;
//        public ThresholdController(IThresholdAppService baseService, IAuthorizationService authorizationService) : base(baseService)
//        {
//            _authorizationService = authorizationService;
//        }

//        [HttpGet]
//        [Route("Threshold-list")]
//        public IActionResult Index()
//        {
//            ViewBag.AuthorizationService = _authorizationService;
//            return View();
//        }
//    }
//}
