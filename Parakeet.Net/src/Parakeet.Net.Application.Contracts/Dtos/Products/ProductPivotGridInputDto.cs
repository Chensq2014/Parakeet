using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Parakeet.Net.Enums;

namespace Parakeet.Net.Dtos.Products
{
    /// <summary>
    /// PivotGridInputDto
    /// </summary>
    public class ProductPivotGridInputDto: InputDateTimeDto
    {

        /// <summary>
        ///  收费类型
        /// </summary>
        [Description("收费类型")]
        public ChargeType? ChargeType { get; set; }
    }
}
