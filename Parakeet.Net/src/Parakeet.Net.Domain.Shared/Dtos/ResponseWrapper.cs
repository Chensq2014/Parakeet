using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 响应包
    /// </summary>
    public class ResponseWrapper
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 总数量
        /// </summary>
        public int Count { get; set; } = 1;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// 消息s
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>
        /// 成功默认对象
        /// </summary>
        public static ResponseWrapper<string> Succeed(string msg = "成功")
        {
            return new ResponseWrapper<string> { Message = msg, Data = msg };
        }

        /// <summary>
        /// 错误默认对象
        /// </summary>
        public static ResponseWrapper<string> Error(string msg = "失败", int code = 400)
        {
            return new ResponseWrapper<string> { Message = msg, Data = msg, Success = false, Code = code, Count = 0 };
        }
    }

    public class ResponseWrapper<T> : ResponseWrapper
    {
        /// <summary>
        /// 返回的泛型数据
        /// </summary>
        public T Data { get; set; }
    }
}
