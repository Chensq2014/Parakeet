using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Parakeet.Net.ValueObjects
{
    /// <summary>
    ///     型号规格
    /// </summary>
    public class Specification : ValueObject
    {
        /// <summary>
        /// 长度
        /// </summary>
        [Range(0, CustomerConsts.MaxValue)]
        public decimal? Length { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [Range(0, CustomerConsts.MaxValue)]
        public decimal? Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Range(0, CustomerConsts.MaxValue)]
        public decimal? Height { get; set; }

        /// <summary>
        /// 厚度
        /// </summary>
        [Range(0, CustomerConsts.MaxValue)]
        public decimal? Thickness { get; set; }

        /// <summary>
        /// 直径
        /// </summary>
        [Range(0, CustomerConsts.MaxValue)]
        public decimal? Diameter { get; set; }

        /// <summary>
        /// 自动属性
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetAtomicValues()
        {
            return new List<object>
            {
                Length,
                Width,
                Height,
                Thickness,
                Diameter
            };
        }
    }
}