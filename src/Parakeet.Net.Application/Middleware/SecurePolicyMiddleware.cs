using Common.Cache;
using Common.Dtos;
using Common.Enums;
using Common.Extensions;
using Common.Helpers;
using Common.Interfaces;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parakeet.Net.Middleware
{
    /// <summary>
    /// 自定义安全策略中间件
    /// </summary>
    public class SecurePolicyMiddleware
    {
        private readonly RequestDelegate _next;
        public SecurePolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        /// <summary>
        /// 委托逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var user = GetUser(context);
            if (user is null)
            {
                Log.Logger.Debug($"{nameof(SecurePolicyMiddleware)}中间件 检测到用户未登录,继续流程让用户跳转登录<br/>");
                //Console.WriteLine($"{nameof(SecurePolicyMiddleware)}中间件 检测到用户未登录,继续流程让用户跳转登录<br/>");
                await _next(context);
            }
            else
            {
                //Console.WriteLine($"{nameof(SecurePolicyMiddleware)}中间件 开始匹配用户{user?.UserName}安全策略");
                Log.Logger.Debug($"{nameof(SecurePolicyMiddleware)}中间件 开始匹配用户{user?.UserName}安全策略");

                //获取客户端请求Ip等信息  nginx 之后，获取出来的ip地址 nginx服务器的，还需要nginx配置 客户端请求ip
                var clientIp = GetRequestIP(context);//"192.168.2.9";//context.Connection.RemoteIpAddress.ToString();//

                var cacheManager = context.RequestServices.GetService<ICacheContainer<MultilevelCache<bool>, bool>>();
                var cachingProvider = context.RequestServices.GetService<IEasyCachingProvider>();
                var cacheKey = $"SecurePolicy:{clientIp}:{user.Id}";
                var validate = await cacheManager.GetCacheValue(cacheKey, () => new MultilevelCache<bool>(cachingProvider, cacheKey, async () =>
                {
                    var pass = true;
                    var policies = await context.RequestServices.GetRequiredService<ISecurePolicyAppService>().GetCurrentUserPolicies(new InputIdDto { Id = user.Id });

                    if (policies.Any())
                    {
                        var clientIpLong = clientIp.IpToNumber();
                        var allowPolicies = policies.Where(m => m.IsAllow).ToList();
                        var rejectPolicies = policies.Where(m => !m.IsAllow).ToList();
                        var allowPass = allowPolicies.Any(m => GetCurrentPolicyValidateResult(m, clientIpLong));
                        var refuse = rejectPolicies.Any(m => GetCurrentPolicyValidateResult(m, clientIpLong));
                        pass = allowPass && !refuse;
                    }
                    return pass;
                })
                {
                    RedisExpireTimeSpan = TimeSpan.FromMinutes(30)
                });

                if (validate)
                {
                    //Console.WriteLine($"{nameof(SecurePolicyMiddleware)} 中间件 匹配用户{user?.UserName}无安全策略或匹配安全策略成功,{clientIp}允许访问");
                    Log.Logger.Debug($"{nameof(SecurePolicyMiddleware)} 中间件 匹配用户{user?.UserName}无安全策略或匹配安全策略成功,{clientIp}允许访问");
                    await _next(context);
                }
                else
                {
                    //Console.WriteLine($"{nameof(SecurePolicyMiddleware)} 中间件 用户{user?.UserName}安全策略 未通过，{clientIp}拒绝访问请求终止!");
                    Log.Logger.Debug($"{nameof(SecurePolicyMiddleware)} 中间件 用户{user?.UserName}安全策略 未通过，{clientIp}拒绝访问请求终止!");
                    //context.Response.Redirect("login跳转登录页");
                    await context.Response.WriteAsync($"{nameof(SecurePolicyMiddleware)}中间件 匹配用户{user?.UserName}安全策略 未通过，{clientIp}拒绝访问请求终止!");
                }

            }

            //Console.WriteLine($"{nameof(SecurePolicyMiddleware)} 中间件 {user?.UserName}安全策略完毕");
        }

        private static bool GetCurrentPolicyValidateResult(SecurePolicyDto policy, long? clientIpLong)
        {
            var validate = policy.IsAllow;
            switch (policy.SecureValidateType)
            {
                case SecureValidateType.None:
                    //todo:ClientOs
                    //validate =  policy.IsAllow;
                    break;
                case SecureValidateType.Ip:
                    validate = policy.StartIpLong <= clientIpLong && clientIpLong <= policy.EndIpLong;
                    break;
                case SecureValidateType.ClientOs:
                    //todo:ClientOs
                    //validate =  policy.IsAllow;
                    break;
                case SecureValidateType.Browser:
                    //todo:Browser
                    //validate =  policy.IsAllow;
                    break;
                case SecureValidateType.DeviceId:
                    //todo:DeviceId
                    //validate =  policy.IsAllow;
                    break;
                default:
                    //validate =  policy.IsAllow;
                    break;
            }

            return validate;
        }

        /// <summary>
        /// 从context中获取user
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private UserDto GetUser(HttpContext context)
        {
            var authResult = context.AuthenticateAsync().Result;
            var principal = authResult?.Principal;
            if (principal?.Claims == null || !principal.Claims.Any())
            {
                return null;
                //context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //throw new KnowException("No Authorization");
            }

            var userInfoStr = principal?.Claims?.FirstOrDefault(t => t.Type.Equals("CurrentUserInfo"))?.Value;
            if (string.IsNullOrWhiteSpace(userInfoStr))
            {
                //context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //throw new KnowException("No UserInfo");
                return null;
            }
            var user = JsonConvert.DeserializeObject<UserDto>(userInfoStr);
            return user;
        }


        /// <summary>
        /// 获取客户端请求Ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetRequestIP(HttpContext context)
        {
            string ip = "";
            try
            {
                IHttpContextAccessor httpContextAccessor = context.RequestServices.GetRequiredService<IHttpContextAccessor>();
                HttpContext httpContext = null;
                if (httpContextAccessor != null)
                {
                    httpContext = httpContextAccessor.HttpContext;
                }

                if (httpContext != null)
                {
                    ip = httpContext.Request.Headers["X-Origin-Request-IP"].FirstOrDefault();
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    }
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = httpContext.Request.Headers["Proxy-Client-IP"].FirstOrDefault();
                    }
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                    }
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = httpContext.Connection.RemoteIpAddress.ToString();
                    }
                    if (!string.IsNullOrEmpty(ip))
                    {
                        ip = ip.Split(",", StringSplitOptions.RemoveEmptyEntries).First().Trim();
#if DEBUG
                        if (ip.Contains("::ffff:"))
                        {
                            ip = ip.Replace("::ffff:", "");
                        }

                        if (ip.Contains("::1"))
                        {
                            ip = LocalIpHelper.GetLocalIpv4();
                        }
#endif
                    }

                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error(ex.Message, ex);
            }

            return ip;
        }


    }
}
