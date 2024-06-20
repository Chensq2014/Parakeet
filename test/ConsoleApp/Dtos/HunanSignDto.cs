using System;

namespace ConsoleApp.Dtos
{
    /// <summary>
    /// 签名
    /// </summary>
    public class HunanSignDto//: ICloneable
    {
        ///// <summary>
        ///// 项目Id
        ///// </summary>
        //public string ProjectId { get; set; } = "5b73796d-7f86-46e3-b743-3d15dfe1ea86";

        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; } = "48efe90b03b947abae25a3ddad170af4";

        /// <summary>
        /// 签名 projectId+json+token的Md5值
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Json对象
        /// </summary>
        public object Body { get; set; }

        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //}
    }
}
