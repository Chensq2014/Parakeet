using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parakeet.Net.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parakeet.Net.Web.Controllers
{
    /// <summary>
    /// 公共管理控制类
    /// </summary>
    public class ManagerController : NetController
    {
        private readonly IAuthorizationService _authorizationService;
        public ManagerController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.AuthorizationService = _authorizationService;
            await Compute().ConfigureAwait(false);//依然会等待子线程计算完毕之后再返回
            return View();
        }


        private async Task Compute()
        {
            await Task.Run(() => {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"第{i}次进入");
                    Thread.Sleep(1000);
                }
            }).ConfigureAwait(false);//依然会等待
        }
    }
}
