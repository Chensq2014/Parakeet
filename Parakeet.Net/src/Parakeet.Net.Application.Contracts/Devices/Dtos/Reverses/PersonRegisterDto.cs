using Parakeet.Net.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Parakeet.Net.Dtos
{
    /// <summary>
    /// 注册人员信息
    /// </summary>
    public class PersonRegisterDto : EquipmentBaseDto
    {
        public override string CommandName => "register_person";

        /// <summary>
        /// 注册人员PersonnelId
        /// </summary>
        public string PersonnelId { get; set; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// IC卡
        /// </summary>
        public string IcCard { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// 名族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 出生日期（yyyy-MM-dd）
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 发证机关
        /// </summary>
        public string IssuedBy { get; set; }

        /// <summary>
        /// 证件有效期起
        /// </summary>
        public string TermValidityStart { get; set; }

        /// <summary>
        /// 证件有效期止
        /// </summary>
        public string TermValidityEnd { get; set; }

        /// <summary>
        /// 身份证照片（base64）
        /// </summary>
        public string IdPhoto { get; set; }

        /// <summary>
        /// 现场人员可见光照片（base64）
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 现场人员红外照片（base64）
        /// </summary>
        public string InfraredPhoto { get; set; }

        /// <summary>
        /// 身份证照片Url
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength512)]
        public string IdPhotoUrl { get; set; }

        /// <summary>
        /// 现场人员可见光照片Url
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength512)]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 现场人员红外照片Url
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength512)]
        public string InfraredPhotoUrl { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 注册人员类型 1采集劳务人员，2采集岗位人员
        /// </summary>
        public int PersonnelType { get; set; }

        /// <summary>
        /// 采集注册类型（3：人脸采集）
        /// </summary>
        public int RegisterType { get; set; }

        /// <summary>
        /// 用户工号（建委下发的）
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerNo { get; set; }

        /// <summary>
        /// 所属企业ID
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength64)]
        public string CorpId { get; set; }

        /// <summary>
        /// 企业编码 CompanyNo
        /// </summary>
        
        [MaxLength(CustomerConsts.MaxLength64)]
        public string CorpCode { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string CorpName { get; set; }

        /// <summary>
        /// 工种ID
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerTypeId { get; set; }

        /// <summary>
        /// 工种编码
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerTypeCode { get; set; }

        /// <summary>
        /// 工种名称 工种 WorkTypeName Job
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerTypeName { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkPostId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkPostCode { get; set; }

        /// <summary>
        /// 岗位名称 AdminPostName AdminPost
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkPostName { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerGroupId { get; set; }

        /// <summary>
        /// 班组编码
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerGroupCode { get; set; }

        /// <summary>
        /// 班组名称 GroupName
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string WorkerGroupName { get; set; }

        /// <summary>
        /// 考勤卡ID
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string AttendanceCardId { get; set; }

        /// <summary>
        /// 考勤卡号
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        public string AttendanceCardNo { get; set; }

        /// <summary>
        /// 是否班组长
        /// </summary>
        public bool GroupLeader { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [MaxLength(20)]
        public string Marital { get; set; }

        /// <summary>
        /// 政治面貌 （ 0.群众 30.中共党员 20.中共预备党员 10.共青团员）
        /// </summary>
        public PoliticsType PoliticsType { get; set; }

        /// <summary>
        /// 是否加入公会 (false否/true是)
        /// </summary>
        public bool? IsJoin { get; set; }

        /// <summary>
        /// 加入公会时间
        /// </summary>
        public DateTime? JoinTime { get; set; }

        /// <summary>
        /// 学历（0=文盲,10=小学 ,20=初中,30=中专,40=高中,CustomerConsts.MaxLength64=大专,60=本科,70=硕士,80=博士）
        /// </summary>
        public EducationType? Education { get; set; }

        /// <summary>
        /// 是否有重病史（false:无 true:有）
        /// </summary>
        public bool HasBadMedicalHistory { get; set; }

        /// <summary>
        /// 是否特种人员(false不是/true是)
        /// </summary>
        public bool IsSpecial { get; set; }
    }
}
