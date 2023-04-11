using Parakeet.Net.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 下发人员信息
    /// </summary>
    public class AddPersonDto : EquipmentBaseDto
    {
        public override string CommandName => "add_person_face";

        /// <summary>
        /// 人员唯一标识
        /// </summary>
         [Description("人员唯一标识")]
         [MaxLength(CustomerConsts.MaxLength36)]
        public string PersonnelId { get; set; }

        /// <summary>
        /// 人员编号
        /// </summary>
         [Description("人员编号")]
         [MaxLength(CustomerConsts.MaxLength64)]
        public string PersonnelCode { get; set; }

        /// <summary>
        /// Utf8编码
        /// 识别通过后会在设备显示屏上显示该名字
        /// </summary>
         [Description("Utf8编码")]
         [MaxLength(CustomerConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 人员类型 1：白名单，2：黑名单，3：访客，4：陌生人
        /// </summary>
         [Description("人员类型 1：白名单，2：黑名单，3：访客，4：陌生人")]
        public FansType FansType { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
         [Description("身份证号码")]
         [MaxLength(CustomerConsts.MaxLength18)]
        public string IdCard { get; set; }

        /// <summary>
        /// Ic卡号
        /// </summary>
         [Description("Ic卡号")]
         [MaxLength(CustomerConsts.MaxLength32)]
        public string IcCard { get; set; }

        /// <summary>
        /// 人脸
        /// </summary>
        [Description("人脸")]
        public virtual List<FaceDto> Faces { get; set; } = new List<FaceDto>();
    }
}
