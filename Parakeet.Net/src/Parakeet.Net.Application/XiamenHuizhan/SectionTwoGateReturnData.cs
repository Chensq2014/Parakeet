using System;
using System.Collections.Generic;

namespace Parakeet.Net.XiamenHuizhan
{
    /// <summary>
    /// 一标段考勤返回数据外层对象 SectionTwoGateReturnData
    /// </summary>
    public class SectionTwoGateReturnData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据结构
        /// </summary>
        public List<SectionTwoGateData> Data { get; set; }
    }

    /// <summary>
    /// SectionTwoGateData
    /// </summary>
    public class SectionTwoGateData
    {
        /// <summary>
        /// 打卡id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 打卡日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 打卡时间
        /// </summary>
        public string ClockedTime { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Identification { get; set; }

        /// <summary>
        /// 班组Id
        /// </summary>
        public long? GroupId { get; set; }

        /// <summary>
        /// 队伍Id
        /// </summary>
        public long? TeamId { get; set; }
        /// <summary>
        /// 工种编码
        /// </summary>
        public string WorkTypeCode { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public long? CompanyId { get; set; }

        /// <summary>
        /// 平台对应分包商ID
        /// </summary>
        public long? VendorId { get; set; }

        /// <summary>
        /// 平台对应班组Id
        /// </summary>
        public long? ProjectWorkerId { get; set; }

        /// <summary>
        /// 项目工人履历ID
        /// </summary>
        public long? LaborWorkerId { get; set; }

        /// <summary>
        /// 是否计算工时 1计算工时，0不计算，默认计算，新产品返回都是1
        /// </summary>
        public int Calculate { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 工人Id
        /// </summary>
        public long? WorkerId { get; set; }

        /// <summary>
        /// 工人姓名
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// 打卡照片
        /// </summary>
        public string ScanPhoto { get; set; }

        /// <summary>
        /// 打卡类型描述
        /// </summary>
        public string ClockTypeDesc { get; set; }

        /// <summary>
        /// 打卡类型
        /// </summary>
        public string ClockType { get; set; }

        /// <summary>
        /// 平台对应队伍id
        /// </summary>
        public long? PlatformGroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PlatformTeamId { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public decimal? Temperature { get; set; }

        /// <summary>
        /// ic卡号
        /// </summary>
        public string IcCard { get; set; }

        /// <summary>
        /// 安全帽编号
        /// </summary>
        public string HatCode { get; set; }

        /// <summary>
        /// 施工区域Id
        /// </summary>
        public long? RegionId { get; set; }

        /// <summary>
        /// 施工区
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 部位id
        /// </summary>
        public long? WorkPointId { get; set; }

        /// <summary>
        /// 部位名称 
        /// </summary>
        public string WorkPointName { get; set; }

        /// <summary>
        /// 围栏id
        /// </summary>
        public long? SegmentId { get; set; }

        /// <summary>
        /// 围栏名称
        /// </summary>
        public string SegmentName { get; set; }

        /// <summary>
        /// 工区Id
        /// </summary>
        public long? AreaId { get; set; }

        /// <summary>
        /// 工区名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// IN进，OUT出
        /// </summary>
        public string InOutType { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public string WorkTypeName { get; set; }

        /// <summary>
        /// 队名
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 钢结构一队
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

    }
}
