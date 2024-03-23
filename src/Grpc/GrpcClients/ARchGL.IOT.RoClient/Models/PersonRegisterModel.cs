﻿using System;

namespace ARchGL.IOT.RoClient.Models
{    
    /// <summary>
    /// 注册人员信息
    /// </summary>
    public class PersonRegisterModel : ModelBase
    {
        public override string CommandName => "register_person";

        /// <summary>
        /// 注册人员PersonnelId
        /// </summary>
        public string PersonnelId { get; set; }

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
        public int Gender { get; set; }

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
        /// <value></value>
        public string IdPhotoUrl { get; set; }

        /// <summary>
        /// 现场人员可见光照片Url
        /// </summary>
        /// <value></value>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 现场人员红外照片Url
        /// </summary>
        /// <value></value>
        public string InfraredPhotoUrl { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 注册人员类型 1采集劳务人员，2采集岗位人员
        /// </summary>
        public int PersonnelType { get; set; } = 1;

        /// <summary>
        /// 采集注册类型（3：人脸采集）
        /// </summary>
        public int RegisterType { get; set; } = 3;

        /// <summary>
        /// 用户工号（建委下发的）
        /// </summary>
        /// <value></value>
        public virtual string WorkerNo { get; set; }

        /// <summary>
        /// 劳务公司ID
        /// </summary>
        /// <value></value>
        public virtual string LaborCompanyId { get; set; }

        /// <summary>
        /// 劳务公司编码
        /// </summary>
        /// <value></value>
        public virtual string LaborCompanyCode { get; set; }

        /// <summary>
        /// 劳务公司名称
        /// </summary>
        /// <value></value>
        public virtual string LaborCompanyName { get; set; }

        /// <summary>
        /// 工种ID
        /// </summary>
        /// <value></value>
        public virtual string WorkerTypeId { get; set; }

        /// <summary>
        /// 工种编码
        /// </summary>
        /// <value></value>
        public virtual string WorkerTypeCode { get; set; }

        /// <summary>
        /// 工种名称 工种 WorkTypeName Job
        /// </summary>
        /// <value></value>
        public virtual string WorkerTypeName { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        /// <value></value>
        public virtual string WorkPostId { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        /// <value></value>
        public virtual string WorkPostCode { get; set; }

        /// <summary>
        /// 岗位名称 AdminPostName AdminPost
        /// </summary>
        /// <value></value>
        public virtual string WorkPostName { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        /// <value></value>
        public virtual string WorkerGroupId { get; set; }

        /// <summary>
        /// 班组编码
        /// </summary>
        /// <value></value>
        public virtual string WorkerGroupCode { get; set; }

        /// <summary>
        /// 班组名称 GroupName
        /// </summary>
        /// <value></value>
        public virtual string WorkerGroupName { get; set; }

        /// <summary>
        /// 考勤卡ID
        /// </summary>
        /// <value></value>
        public virtual string AttendanceCardId { get; set; }

        /// <summary>
        /// 考勤卡号
        /// </summary>
        /// <value></value
        public virtual string AttendanceCardNo { get; set; }

        /// <summary>
        /// 是否班组长
        /// </summary>
        /// <value></value>
        public virtual bool GroupLeader { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public virtual string Marital { get; set; }

        /// <summary>
        /// 政 治 面 貌 （ 0.中 共 党 员 1. 中共 预 备 党 员 2.共 青 团 员 3. 群 众）
        /// </summary>
        public virtual int? PoliticsType { get; set; } = 3;

        /// <summary>
        /// 是 否 加 入 公 会 (1 是，0 否)
        /// </summary>
        public virtual int? IsJoin { get; set; } = 0;

        /// <summary>
        /// 加入公会时间
        /// </summary>
        public virtual DateTime? JoinTime { get; set; }

        /// <summary>
        /// 学 历 （ 0= 小学 ,1= 初 中,2= 高 中,3= 中 专,4= 大 专,5= 本 科,6= 硕 士,7= 博 士,8= 文盲）
        /// </summary>
        public virtual int? Education { get; set; }

        /// <summary>
        /// 是 否 有 重 病 史（0:无,1:有）
        /// </summary>
        public virtual int? HasBadMedicalHistory { get; set; } = 0;

        /// <summary>
        /// 是否特种人员(true 是/false 不是)
        /// </summary>
        /// <value></value>
        public virtual bool IsSpecial { get; set; }
    }
}