using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Parakeet.NetCore.Web.ViewComponents
{
    public class MenuNavigationViewComponent : ViewComponent
    {
        private IConfiguration configuration;
        private IAuthorizationService _authorizationService;
        public MenuNavigationViewComponent(IConfiguration configuration, IAuthorizationService authorizationService)
        {
            this.configuration = configuration;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                Console.WriteLine("MenuNavigationViewComponent Initialize");
            });

            ViewData["Authority"] = _authorizationService;//configuration.GetValue<string>("Authority");
            return View();
        }
    }
}
