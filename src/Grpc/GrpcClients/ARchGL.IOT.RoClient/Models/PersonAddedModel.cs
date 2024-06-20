using System.Collections.Generic;

namespace ARchGL.IOT.RoClient.Models
{
    public class PersonAddedModel : ModelBase
    {
        public override string CommandName => "add_person_face";

        /// <summary>
        /// 人员唯一标识Id
        /// </summary>
        /// <value></value>
        public string PersonnelId { get; set; }

        /// <summary>
        /// 人员编号
        /// </summary>
        /// <value></value>
        public string PersonnelCode { get; set; }

        /// <summary>
        /// 名字[必填]
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// 人员类型
        /// 1：白名单，2：黑名单，3：访客，4：陌生人
        /// </summary>
        /// <value></value>
        public int Type { get; set; } = 1;

        /// <summary>
        /// 身份证号码[必填]
        /// </summary>
        /// <value></value>
        public string IdCard { get; set; }

        /// <summary>
        /// Ic卡号
        /// </summary>
        /// <value></value>
        public string IcCard { get; set; }

        /// <summary>
        /// 人脸[必填]
        /// </summary>
        public List<FaceModel> Faces { get; set; } = new List<FaceModel>();
    }
}