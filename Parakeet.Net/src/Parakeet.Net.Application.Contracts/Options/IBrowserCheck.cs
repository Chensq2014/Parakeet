using Microsoft.AspNetCore.Http;
using Parakeet.Net.Dtos;
using System;

namespace Parakeet.Net.Options
{
    /// <summary>
    /// 浏览器检查接口
    /// </summary>
    public interface IBrowserCheck
    {
        Tuple<bool, string> CheckBrowser(HttpContext httpContext,BrowserFilterOptions options);
    }
}
