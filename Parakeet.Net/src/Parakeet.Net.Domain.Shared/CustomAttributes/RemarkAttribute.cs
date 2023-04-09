using System;

namespace Parakeet.Net.CustomAttributes
{
    public class RemarkAttribute : Attribute
    {
        public RemarkAttribute(string remark)
        {
            Remark = remark;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
