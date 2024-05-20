using System;

namespace Parakeet.Net.ROClient.Models
{
    public class PersonLocationBasicAddedModel : ModelBase
    {
        public override string CommandName => "add_personlocation_basic";

        /// <summary>
        /// 传感器Id
        /// </summary>
        public int SensorId { get; set; } = 1;

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工人Id
        /// </summary>
        public Guid? WorkerId { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public string WorkerType { get; set; }

        /// <summary>
        /// 注册人员类型
        /// </summary>
        public int? PersonnelType { get; set; }

        /// <summary>
        /// 标记
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImage { get; set; }
    }
}