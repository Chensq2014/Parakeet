using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Parakeet.Net.Controllers
{
    /// <summary>
    /// 自定义鉴权测试
    /// </summary>
    [AllowAnonymousAttribute]
    [Route("/api/parakeet/authentication/[action]")]
    public class AuthenticationController : NetController
    {
        private IConfiguration _iConfiguration => LazyServiceProvider.LazyGetRequiredService<IConfiguration>();


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            Logger.LogWarning("This is AuthenticationController-Index 1");

            var endpoint = base.HttpContext.GetEndpoint();
            Console.WriteLine(base.HttpContext.Items["__AuthorizationMiddlewareWithEndpointInvoked"]);

            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}   标签：{base.HttpContext.Items["__AuthorizationMiddlewareWithEndpointInvoked"]}";

            return View();
        }

        #region 基于Cookie基本鉴权-授权基本流程

        [HttpGet]
        [AllowAnonymous]
        [Route("loginRole")]
        public async Task<IActionResult> Login(string name, string password, string role = "admin")
        {
            //base.HttpContext.RequestServices.
            //IAuthenticationService

            if ("Parakeet".Equals(name, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals("123456"))
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
                //claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "290668617@qq.com"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, role));

            //await base.HttpContext.SignInAsync("CustomScheme", 
            await base.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), 
                new AuthenticationProperties
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
        public async Task<IActionResult> Logout()
        {
            await base.HttpContext.SignOutAsync("CustomScheme");
            await base.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        /// <summary>
        /// 认证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Authentication()
        {
            var result = await base.HttpContext.AuthenticateAsync("CustomScheme");
            if (result?.Principal != null)
            {
                base.HttpContext.User = result.Principal;
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证成功，包含用户{base.HttpContext.User.Identity.Name}"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
        }


        /// <summary>
        /// 需要授权的页面
        /// http://localhost:23517/api/parakeet/authentication/InfoWithAuthorize
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]//需要鉴权+授权--
        public IActionResult InfoWithAuthorize()
        {
            Logger.LogWarning("This is Authentication-InfoWithAuthorize 1");

            var endpoint = base.HttpContext.GetEndpoint();
            base.ViewBag.Info = $"endpoint:{endpoint.DisplayName}   标签：{base.HttpContext.Items["__AuthorizationMiddlewareWithEndpointInvoked"]}";

            return View();
        }




        #endregion


        #region 自定义UrlToken

        /// <summary>
        /// http://localhost:23517/api/parakeet/Authentication/UrlToken
        /// http://localhost:23517/api/parakeet/Authentication/UrlToken?UrlToken=Parakeet
        /// </summary>
        /// <returns></returns>
        //[Authorize(AuthenticationSchemes= CommonConsts.CustomAuthenticationScheme)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UrlToken()
        {
            Console.WriteLine($"请求进入UrlToken，表明鉴权授权已通过");
            Console.WriteLine($"当然进行主动鉴权");

            var result = await base.HttpContext.AuthenticateAsync(CommonConsts.CustomAuthenticationScheme);

            if (result?.Principal == null)//不会发生
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                base.HttpContext.User = result.Principal;
            }

            //主动授权检测
            var user = base.HttpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                if (!user.Identity.Name.Equals("Parakeet", StringComparison.OrdinalIgnoreCase))
                {
                    await base.HttpContext.ForbidAsync(CommonConsts.CustomAuthenticationScheme);
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
                    });
                }
                else
                {
                    //有权限
                    return new JsonResult(new
                    {
                        Result = true,
                        Message = $"授权成功，正常访问页面！",
                        Html = "Hello Root!"
                    });
                }
            }
            else
            {
                await base.HttpContext.ChallengeAsync(CommonConsts.CustomAuthenticationScheme);
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"授权失败，没有登录"
                });
            }
        }


        /// <summary>
        /// http://localhost:23517/api/parakeet/Authentication/AuthorizeData
        /// http://localhost:23517/api/parakeet/Authentication/AuthorizeData?UrlToken=Parakeet
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AuthorizeData()
        {
            await base.HttpContext.SignOutAsync(CommonConsts.CustomAuthenticationScheme);
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        #endregion




    }
}
