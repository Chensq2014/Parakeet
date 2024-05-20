using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace ConsoleApp.Dtos.XiamenHuizhan
{
    /// <summary>
    /// 考勤(多标段综合)
    /// </summary>
    public class GateCommon : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 考勤
        /// </summary>
        public GateCommon() { }

        /// <summary>
        /// 考勤
        /// </summary>
        /// <param name="id"></param>
        public GateCommon(Guid id) : base(id) { }

        /// <summary>
        /// 设备编码（设备真实设备序列号）
        /// </summary>
        [Description("设备编码")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 转发编码（对外提供设备序列号）
        /// </summary>
        [Description("转发编码")]
        public string FakeNo { get; set; }

        /// <summary>
        /// 人员唯一标识
        /// </summary>
        public string PersonnelId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Description("名字")]
        public string PersonnelName { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        [Description("用户工号")]
        public string WorkerNo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(18)]
        [Description("身份证号")]
        public string IdCard { get; set; }

        /// <summary>
        /// 进出状态 【1-进 IN】 【2-出 OUT】
        /// </summary>
        [Description("进出状态 【1-进 IN】 【2-出 OUT】")]
        public string InOutType { get; set; }

        /// <summary>
        /// 设备类型  GATEFACE
        /// </summary>
        [Description("设备类型")]
        public string DeviceType { get; set; }

        /// <summary>
        /// 性别 0 /1 
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 考勤照片
        /// </summary>
        [Description("考勤照片")]
        public string Photo { get; set; }

        /// <summary>
        /// 考勤照片Url
        /// </summary>
        [Description("考勤照片Url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [Description("项目编号")]
        public string ProjectSysNo { get; set; }

        /// <summary>
        /// 分包商编码
        /// </summary>
        [Description("分包商编码")]
        public string SubContractorSysNo { get; set; }

        /// <summary>
        /// 分包商名称
        /// </summary>
        [Description("分包商名称")]
        public string SubContractorName { get; set; }

        /// <summary>
        /// 队伍Id
        /// </summary>
        [Description("队伍Id")]
        public string TeamId { get; set; }

        /// <summary>
        /// 队伍号
        /// </summary>
        [Description("队伍号")]
        public string TeamSysNo { get; set; }

        /// <summary>
        /// 队伍名称
        /// </summary>
        [Description("队伍名称")]
        public string TeamName { get; set; }

        /// <summary>
        /// 主人Id
        /// </summary>
        [Description("主人Id")]
        public string VendorId { get; set; }

        /// <summary>
        /// 项目工人Id
        /// </summary>
        [Description("项目工人Id")]
        public string ProjectWorkerId { get; set; }

        /// <summary>
        /// 劳务工人Id
        /// </summary>
        [Description("劳务工人Id")]
        public string LaborWorkerId { get; set; }

        /// <summary>
        /// 班组Id
        /// </summary>
        [Description("班组")]
        public string GroupId { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        [Description("班组名称")]
        public string GroupName { get; set; }

        /// <summary>
        /// 组Id
        /// </summary>
        [Description("组Id")]
        public string PlatformGroupId { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        [Description("组名")]
        public string PlatformGroupName { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        [Description("公司Id")]
        public string CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [Description("公司名称")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 工种编码
        /// </summary>
        [Description("工种编码")]
        public string WorkTypeCode { get; set; }

        /// <summary>
        /// 工种名称
        /// </summary>
        [Description("工种名称")]
        public string WorkTypeName { get; set; }

        /// <summary>
        /// 识别通道
        /// </summary>
        [Description("检查通道")]
        public string CheckChannel { get; set; }

        /// <summary>
        /// 相似度
        /// </summary>
        [Description("相似度")]
        public decimal? FaceSimilarity { get; set; }

        /// <summary>
        /// RecordTime
        /// </summary>
        public DateTime? RecordTime { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Description("经度")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>

        [Description("纬度")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        [Description("区域Id")]
        public string RegionId { get; set; }

        /// <summary>
        /// 施工区域
        /// </summary>
        [Description("施工区域")]
        public string RegionName { get; set; }

        /// <summary>
        /// 段Id
        /// </summary>
        [Description("段Id")]
        public string SegmentId { get; set; }

        /// <summary>
        /// 段名
        /// </summary>
        [Description("段名")]
        public string SegmentName { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        [Description("区域Id")]
        public string AreaId { get; set; }

        /// <summary>
        /// 项目区域
        /// </summary>
        [Description("项目区域")]
        public string AreaName { get; set; }

        /// <summary>
        /// 生活区域Id
        /// </summary>
        [Description("生活区域Id")]
        public string WorkPointId { get; set; }

        /// <summary>
        /// 生活区域门禁
        /// </summary>
        [Description("生活区域门禁")]
        public string WorkPointName { get; set; }
    }
}
