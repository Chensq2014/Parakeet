using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Parakeet.Net.ValueObjects
{
    /// <summary>
    ///     建筑面积
    /// </summary>
    public class Area : ValueObject
    {
        /// <summary>
        ///     建筑面积
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength255)]
        public string FloorArea { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return FloorArea;
            yield return Total;
            yield return Overground;
            yield return Underground;
        }

        #region  额外设计的数值字段

        /// <summary>
        ///     总面积
        /// </summary>
        [Range(0, CustomerConsts.MaxNumber)]
        public decimal? Total { get; set; }

        /// <summary>
        ///     地上面积
        /// </summary>
        [Range(0, CustomerConsts.MaxNumber)]
        public decimal? Overground { get; set; }

        /// <summary>
        ///     地下面积
        /// </summary>
        [Range(0, CustomerConsts.MaxNumber)]
        public decimal? Underground { get; set; }

        #endregion
    }
}