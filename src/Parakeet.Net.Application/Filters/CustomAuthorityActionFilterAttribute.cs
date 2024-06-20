using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Linq;
using Common.Enums;
using Common.Extensions;
using Volo.Abp.Identity;

namespace Parakeet.Net.Filters
{
    /// <summary>
    /// Action上的Filter,Mvc5项目搬移到netcore中的权限权限检测专用
    /// netcore扩展的是再action利用管道的方式添加aop动作，而非mvc的aop操作
    /// </summary>
    public class CustomAuthorityActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly ILogger<CustomAuthorityActionFilterAttribute> _logger = null;
        private readonly string _loginUrl = null;
        public CustomAuthorityActionFilterAttribute(ILogger<CustomAuthorityActionFilterAttribute> logger, 
            IModelMetadataProvider metadataProvider, string loginUrl = "~/Account/LogIn")
        {
            this._logger = logger;
            _metadataProvider = metadataProvider;
            _loginUrl = loginUrl;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this._logger.LogDebug($"{nameof(CustomAuthorityActionFilterAttribute)} Executing!");
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                //var isDefined = controllerActionDescriptor.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute),true);
                var isDefined = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(true).Any(a => a.GetType() == typeof(AllowAnonymousAttribute));
                if (isDefined)
                {
                    _logger.LogDebug($"访问 {context.HttpContext.Request.GetDisplayUrl()}  支持匿名");
                    //return;//支持匿名属性的controller与action，立即返回,什么也不干,继续去执行action
                }
                else
                {
                    string userString = context.HttpContext.Session.GetString("CurrentUser");
                    if (userString.HasValue())
                    {
                        var currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<IdentityUserDto>(userString);

                        this._logger.LogDebug($"CustomAuthorityActionFilter 权限检查通过 {currentUser?.Name}登陆了系统!");
                        //return;//用户登陆了，就啥也不干，继续去执行action
                    }
                    else
                    {
                        //1、ajax请求  应该返回一个Json
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
                }
            }
        }


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this._logger.LogDebug($"{nameof(CustomAuthorityActionFilterAttribute)}  Executed!");
        }


    }
}
