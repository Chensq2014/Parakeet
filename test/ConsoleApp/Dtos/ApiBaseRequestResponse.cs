using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 请求返回基类
    /// </summary>
    public class ApiBaseRequestResponse
    {
        public int Errcode { get; set; }
        public string Errmsg { get; set; }
        public string Content { get; set; }
    }
}
