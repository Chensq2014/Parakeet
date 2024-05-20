using System;
using System.Collections.Generic;

namespace Parakeet.Net.XiamenHuizhan
{
    /// <summary>
    /// 一标段考勤返回数据外层对象 SectionOneGateReturnData
    /// </summary>
    public class SectionOneGateReturnData
    {
        /// <summary>
        /// 返回编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回根节点数据
        /// </summary>
        public SectionOneGateRootData Data { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// SectionOneGateRootData
    /// </summary>
    public class SectionOneGateRootData
    {
        /// <summary>
        /// 数据结构
        /// </summary>
        public List<SectionOneGateData> Data { get; set; }

        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// EnableSolrQuery
        /// </summary>
        public bool EnableSolrQuery { get; set; }

        /// <summary>
        /// PassDataPermissionAuth
        /// </summary>
        public bool PassDataPermissionAuth { get; set; }

        /// <summary>
        /// OnlyTopAndSelf
        /// </summary>
        public bool OnlyTopAndSelf { get; set; }

        /// <summary>
        /// PageIndex
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Draw
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// RecordsTotal
        /// </summary>
        public long RecordsTotal { get; set; }

        /// <summary>
        /// PassFilterDeletedStatus
        /// </summary>
        public bool PassFilterDeletedStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// SectionOneGateData
    /// </summary>
    public class SectionOneGateData
    {
        /// <summary>
        /// SubContractorSysNo
        /// </summary>
        public string SubContractorSysNo { get; set; }

        /// <summary>
        /// WorkerSysNo
        /// </summary>
        public string WorkerSysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CheckChannel { get; set; }

        /// <summary>
        /// TeamSysNo
        /// </summary>
        public string TeamSysNo { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime? Time { get; set; }

        ///// <summary>
        ///// TimeFormat
        ///// </summary>
        //public DateTime? TimeFormat => DateTime.ParseExact(Time, "yyyy/MM/dd", CultureInfo.CurrentCulture);

        /// <summary>
        /// Gender
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        ///  TeamName eg:升恒监理
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// SubContractorName eg:福建升恒建设集团有限公司
        /// </summary>
        public string SubContractorName { get; set; }

        /// <summary>
        /// Image 
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// WorkerName
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// ProjectSysNo
        /// </summary>
        public string ProjectSysNo { get; set; }

        /// <summary>
        /// Type 进出类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// InDate
        /// </summary>
        public DateTime? InDate { get; set; }

        ///// <summary>
        ///// InDateFormat
        ///// </summary>
        //public DateTime? InDateFormat => DateTime.ParseExact(InDate, "yyyy/MM/dd", CultureInfo.CurrentCulture);

        /// <summary>
        /// SerialNumber
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// IDCardNumber
        /// </summary>
        public string IDCardNumber { get; set; }

        /// <summary>
        /// FaceSimilarity
        /// </summary>
        public decimal? FaceSimilarity { get; set; }

    }
}
