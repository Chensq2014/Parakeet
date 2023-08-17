using Parakeet.Net.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parakeet.Net.Entities
{
    /// <summary>
    /// 劳务人员
    /// </summary>
    [Description("劳务人员")]
    [Table("Parakeet_Workers", Schema = "parakeet")]
    public class Worker : BaseEntity
    {
        public Worker()
        {
        }

        public Worker(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 身份证号
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength18)]
        [Description("身份证号")]
        public string IdCard { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(CustomerConsts.MaxLength18)]
        [Description("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 性别（ 1： 男， 2： 女）
        /// </summary>
        [Description(" 性别（ 1： 男， 2： 女）")]
        public GenderType Gender { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength18)]
        [Description("民族")]
        public string Nation { get; set; }

        /// <summary>
        /// 出生日期（ yyyy-MM-dd）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength16)]
        [Description("出生日期（ yyyy-MM-dd）")]
        public string Birthday { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        [Description("住址")]
        public string Address { get; set; }

        /// <summary>
        /// 发证机关
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength64)]
        [Description("发证机关")]
        public string IssuedBy { get; set; }

        /// <summary>
        /// 证件有效期起，格式: 20010101
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength11)]
        [Description("证件有效期起，格式: 20010101")]
        public string TermValidityStart { get; set; }

        /// <summary>
        /// 证件有效期止，格式: 20010101
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength11)]
        [Description("证件有效期止，格式: 20010101")]
        public string TermValidityEnd { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength11)]
        [Description("联系电话")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 身份证照片（base64）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        [Description("身份证照片（base64）")]
        public string IdPhoto { get; set; }

        /// <summary>
        /// 现场人员可见光照片（base64）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        [Description("现场人员可见光照片（base64）")]
        public string Photo { get; set; }

        /// <summary>
        /// 现场人员红外照片（base64）
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength8192)]
        [Description("现场人员红外照片（base64）")]
        public string InfraredPhoto { get; set; }

        /// <summary>
        /// 身份证照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        [Description("身份证照片Url")]
        public string IdPhotoUrl { get; set; }

        /// <summary>
        /// 现场人员可见光照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        [Description("现场人员可见光照片Url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 现场人员红外照片Url
        /// </summary>
        [MaxLength(CustomerConsts.MaxLength512)]
        [Description("现场人员红外照片Url")]
        public string InfraredPhotoUrl { get; set; }

        #endregion

        #region 设备人员
        /// <summary>
        /// 人员所在设备集合
        /// </summary>
        public virtual ICollection<DeviceWorker> DeviceWorkers { get; set; } = new HashSet<DeviceWorker>();
        #endregion
    }
}