using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// 自定义授权
    /// </summary>
    public class CustomAuthorizationFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorizationFilterAttribute> _logger = null;
        private readonly string _loginUrl = null;
        public CustomAuthorizationFilterAttribute(
            ILogger<CustomAuthorizationFilterAttribute> logger,
             string loginUrl = "~/Account/LogIn")
        {
            this._logger = logger;
            _loginUrl = loginUrl;
        }

        /// <summary>
        /// 发生在请求刚进入MVC流程，还没实例化控制器
        /// 检测用户登陆--以及是否有权限
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Console.WriteLine($"This is {nameof(CustomAuthorizationFilterAttribute)} .OnAuthorizationAsync");

            //throw new Exception("CustomAuthorizationFilter Exception");
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))//Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter
            {
                return;//匿名特性AllowAnonymousAttribute 会生成Filter
            }
            if (context.ActionDescriptor.EndpointMetadata.Any(item => item is IAllowAnonymous))//鉴权授权的Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute
            {
                return;//匿名就不检测，直接继续
            }

            string user = context.HttpContext.Request.Query["UserName"];
            //var memberValidation = HttpContext.Current.Request.Cookies.Get("CurrentUser");//使用cookie
            //也可以使用数据库、nosql等介质
            //context.HttpContext.GetCurrentUserBySession();//读取用户信息
            if (string.IsNullOrEmpty(user))
            {
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")//.IsAjaxRequest()
                {
                    var ajaxResult = new
                    {
                        Result = DoResult.NoAuthorization,
                        Status = false,
                        Msg = "没有登陆，无权访问"
                    };
                    context.Result = new JsonResult(ajaxResult);
                }
                else
                {
                    //2、跳转登录页面   
                    //短路器--停止当前流程，去别的执行流程
                    _logger.LogDebug($"访问 {context.HttpContext.Request.GetDisplayUrl()} 没有用户");
                    //打开A页面--没有登陆---跳转到登陆页---希望登陆后，再跳到刚才的页面
                    //跳转到登录页前用session保存一下用户访问的这个页面，用户登录后再跳转回这个原始访问页面
                    context.HttpContext.Session.SetString("CurrentUrl", context.HttpContext.Request.GetDisplayUrl());
                    context.Result = new RedirectResult(_loginUrl); ;//短路器，权限坚持没通过就跳转登录页
                }
            }
            else
            {
                Console.WriteLine($"This is {user}访问系统");
            }
            await Task.CompletedTask;
        }
    }
}
