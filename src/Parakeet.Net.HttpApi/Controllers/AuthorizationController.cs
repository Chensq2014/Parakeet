using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 自定义授权测试
    /// </summary>
    [Route("/api/parakeet/authorization/[action]")]
    public class AuthorizationController : NetController
    {
        /// <summary>
        /// 需要授权的页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies")]//
        public IActionResult Info()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Infoadmin()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult InfoUser()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin,User")]
        public IActionResult InfoadminUser()
        {
            return View();
        }

        #region Policy
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "adminPolicy")]
        public IActionResult InfoadminPolicy()
        {
            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "UserPolicy")]
        public IActionResult InfoUserPolicy()
        {
            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "QQEmail")]
        public IActionResult InfoQQEmail()
        {
            return View();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Cookies", Policy = "DoubleEmail")]
        public IActionResult InfoDoubleEmail()
        {
            return View();
        }
        #endregion


        #region CustomScheme
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomScheme(string name, string password)
        {
            //base.HttpContext.RequestServices.
            //IAuthenticationService

            if ("ParakeetCustomScheme".Equals(name, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals("123456"))
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
                await base.HttpContext.SignInAsync("CustomScheme", new ClaimsPrincipal(claimIdentity), new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });//登录为默认的scheme  cookies
                return new JsonResult(new
                {
                    Result = true,
                    Message = "登录成功"
                });
            }
            else
            {
                await Task.CompletedTask;
                return new JsonResult(new
                {
                    Result = false,
                    Message = "登录失败"
                });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutCustomScheme()
        {
            await base.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        /// <summary>
        /// 需要授权的页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "CustomScheme")]
        public IActionResult InfoCustomScheme()
        {
            return View();
        }
        #endregion



        #region jwt 测试

        /// <summary>
        /// 就是个简单的webapi---带授权要求，也就是JWT
        /// 1  特性标记
        /// 2  Add+Use
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/Auth/InfoWithAuth")]
        [Authorize]//带个Token就算数---需要灵活授权，集合JWT来拓展下
        public IActionResult InfoWithAuth()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithAuth OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleAdmin")]
        [Authorize(Roles = "Admin")]
        public IActionResult InfoWithRoleAdmin()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleAdmin OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleUser")]
        [Authorize(Roles = "User")]
        public IActionResult InfoWithRoleUser()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleUser OK!"
            });
        }

        [HttpGet]
        [Route("/Auth/InfoWithRoleAdminOrUser")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult InfoWithRoleAdminOrUser()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithRoleAdminOrUser OK!"
            });
        }

        //复杂的
        [HttpGet]
        [Route("/Auth/InfoWithComplicated")]
        [Authorize(Policy = "ComplicatedPolicy")]
        public IActionResult InfoWithComplicated()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithComplicated OK!"
            });
        }

        //动态校验的需求

        [HttpGet]
        [Route("/Auth/InfoWithDB")]
        [Authorize(Policy = "DBPolicy")]
        public IActionResult InfoWithDB()
        {
            return new JsonResult(new
            {
                Result = true,
                Message = "This is InfoWithDB OK!"
            });
        }
        #endregion
    }
}
