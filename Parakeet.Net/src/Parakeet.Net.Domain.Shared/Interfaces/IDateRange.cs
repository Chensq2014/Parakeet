using System;

namespace Parakeet.Net.Interfaces
{
    /// <summary>
    /// 时间范围接口
    /// </summary>
    public interface IDateRange
    {
        /// <summary>
        ///     开始时间
        /// </summary>
        DateTime? StartDate { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        DateTime? EndDate { get; set; }
    }
}
