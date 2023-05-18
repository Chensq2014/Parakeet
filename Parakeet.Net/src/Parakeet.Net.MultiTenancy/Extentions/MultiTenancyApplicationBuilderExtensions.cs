using Microsoft.AspNetCore.Builder;
using System;

namespace Parakeet.Net.MultiTenancy
{
    public static class MultiTenancyApplicationBuilderExtensions
    {
        /// <summary>
        /// 自定义中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomMultiTenancy(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomMultiTenancyMiddleware>(Array.Empty<object>());
        }
    }
}
