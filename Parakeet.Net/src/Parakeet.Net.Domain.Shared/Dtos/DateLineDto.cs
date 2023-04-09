using System;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 时间轴dto
    /// </summary>
    public class DateLineDto : DateLineDto<double?>
    {
    }
    public class DateLineDto<T>
    {
        public Guid? ParentId { get; set; }

        public Guid? Id { get; set; }

        public string Name { get; set; }

        public T Amount { get; set; }

        public T Custom { get; set; }

        public DateTime? Time { get; set; }
    }
}
