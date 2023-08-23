using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 浏览器配置信息 无参构造函数
    /// </summary>
    public class BrowserFilterOptions
    {
        public bool EnableIE { get; set; }
        public bool EnableEdge { get; set; }
        public bool EnableChorme { get; set; }
        public bool EnableFirefox { get; set; }

        List<Func<HttpContext, Tuple<bool, string>>> DisableList =
            new List<Func<HttpContext, Tuple<bool, string>>>();

        public void InitDisableList(Func<HttpContext, Tuple<bool, string>> func)
        {
            DisableList.Add(func);
        }
    }
}
