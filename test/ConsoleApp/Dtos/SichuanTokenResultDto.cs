using System;
using System.Collections.Generic;

namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 省厅请求token返回类型
    /// </summary>
    public class SichuanTokenResultDto
    {
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// state
        /// </summary>
        public string State { get; set; }

        public SichuanTokenResultDataDto Data { get; set; }

    }
}
