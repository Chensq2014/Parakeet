using Microsoft.AspNetCore.Http;

namespace Parakeet.Net.Extentions
{
    /// <summary>
    /// 浏览器设置扩展
    /// </summary>
    public class UserAgentExtention
    {
        /// <summary>
        /// 检查cookie设置 是否跨站点
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="options"></param>
        public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite < SameSiteMode.Lax)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Lax;//可跨站点cookie
                }
            }
        }

        /// <summary>
        /// 根据浏览器判断是否更改cookie
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static bool DisallowsSameSiteNone(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return false;
            }

            if (userAgent.Contains("CPU iPhone OS 12") ||
                userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }
            return userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6");
        }
    }
}
