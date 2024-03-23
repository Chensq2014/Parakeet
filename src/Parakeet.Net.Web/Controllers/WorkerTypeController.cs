using Common.Entities;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Controllers;

namespace Parakeet.Net.Web.Controllers
{
    //[Authorize(WorkerTypePermissions.WorkerType.Default)]
    [Route("/api/parakeet/[controller]/[action]")]
    public class WorkerTypeController : BaseEntityController<WorkerType>
    {
        //private readonly IAuthorizationService _authorizationService;
        public WorkerTypeController(IWorkerTypeAppService baseService) : base(baseService)
        {
            //_authorizationService = authorizationService;
        }

        [HttpGet]
        //[Authorize(WorkerTypePermissions.WorkerType.Default)]
        public IActionResult Index()
        {
            //ViewBag.AuthorizationService = _authorizationService;
            return View();
        }
    }
}
