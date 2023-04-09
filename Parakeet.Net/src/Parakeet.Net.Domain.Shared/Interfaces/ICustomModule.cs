using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// 自定义Module接口
    /// </summary>
    public interface ICustomModule
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 模块所在区域
        /// </summary>
        string Area { get; }

        /// <summary>
        /// 模块排序字段
        /// </summary>
        int Order { get; }
    }
}
