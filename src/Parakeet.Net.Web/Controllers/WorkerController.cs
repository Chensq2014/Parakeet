//using Common.Entities;
//using Common.Interfaces;
//using Microsoft.AspNetCore.Mvc;
//using Parakeet.Net.Controllers;

//namespace Parakeet.Net.Web.Controllers
//{
//    [Route("/api/parakeet/[controller]/[action]")]
//    //[Authorize(WorkerPermissions.Worker.Default)]
//    public class WorkerController : BaseParakeetEntityController<Worker>
//    {
//        //private readonly IAuthorizationService _authorizationService;
//        public WorkerController(IWorkerAppService baseService) : base(baseService)
//        {
//            //_authorizationService = authorizationService;
//        }

//        [HttpGet]
//        //[Authorize(WorkerPermissions.Worker.Default)]
//        public IActionResult Index()
//        {
//            //ViewBag.AuthorizationService = _authorizationService;
//            return View();
//        }
//    }
//}
