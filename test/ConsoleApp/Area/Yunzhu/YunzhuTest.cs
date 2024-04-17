using ConsoleApp.Dtos;
using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.HttpClients;
using Newtonsoft.Json;
using Common.Dtos;
using Common.Encrypts;
using Common.Extensions;

namespace ConsoleApp.Area.Yunzhu
{
    /// <summary>
    /// 云筑测试
    /// </summary>
    public static class YunzhuTest
    {
        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "SupplierKeyId",
            SupplierKeySecret = "SupplierKeySecret",
            ProjectKeyId = "SP.58464811199a4fd78eaeac0afe8273cc",
            ProjectKeySecret = "6ccd6d850ba543328d26173b777d0f92"
        };

        #region 获取token

        /// <summary>
        /// 软件服务商统一社会信用代码
        /// </summary>
        public static string UnitCode = "915001060891260711";

        /// <summary>
        /// 软件服务商统一社会信用代码
        /// </summary>
        public static string AppId = "SP.58464811199a4fd78eaeac0afe8273cc";

        /// <summary>
        /// 软件服务商统一社会信用代码
        /// </summary>
        public static string AppSecret = "6ccd6d850ba543328d26173b777d0f92";

        /// <summary>
        /// 重庆筑智建科技(重庆)科技有限公司
        /// </summary>
        public static string ProjectName = "重庆筑智建科技(重庆)科技有限公司";

        /// <summary>
        /// Host
        /// </summary>
        public static string Host = "https://ibuildapi.yzw.cn/open.api";

        /// <summary>
        /// Port
        /// </summary>
        public static string Port = "443";


        #endregion

        #region 上传进度

        /// <summary>
        /// 获取上传进度数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetProcessTaskDataContent()
        {
            var guid = Guid.NewGuid();
            var input = new
            {
                projectSysNo = 12345678,// integer 是   项目编号；怎么获取项目编号？
                processPlanId = guid.ToString(),// string  是   进度计划Id
                processPlanName = "进度计划名称",// string  是   进度计划名称
                sourceId = guid.ToString(),// string  是   节点Id，数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                parentSourceId = guid.ToString(),//  string  否   父级节点Id，无父级则为null
                name = "节点名称",// string  是   节点名称
                level = 1,// integer 是   层级级别
                sortIndex = 1,// integer 是   排序字段，用于同一级别节点排序，按照升序排序
                leaf = false,// boolean 是   是否叶子节点
                responsibleUserId = "1/2/3",//  string  否   责任人Id，多个以“/”分隔，例如：5 / 21 / 33
                responsibleUserName = "张三 / 李四 / 王五",//  string  否   责任人名称，多个以“/”分隔，必须与责任人Id一一对应，例如：张三 / 李四 / 王五
                submitUserId = "1/2/3",// string  否   填报人Id，多个以“/”分隔，例如：5 / 21 / 33
                submitUserName = "张三 / 李四 / 王五",// string  否   填报人名称，多个以“/”分隔，必须与填报人Id一一对应，例如：张三 / 李四 / 王五
                planStartTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string  是   计划开始时间，格式：yyyy - MM - dd HH:mm: ss
                planEndTime = $"{DateTime.Now.AddDays(100):yyyy-MM-dd HH:mm:ss}",// string 是   计划结束时间，格式：yyyy - MM - dd HH: mm: ss
                actualStartTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string 否   实际开始时间，格式：yyyy - MM - dd HH: mm: ss
                actualEndTime = $"{DateTime.Now.AddDays(90):yyyy-MM-dd HH:mm:ss}",//  string 否   实际结束时间，格式：yyyy - MM - dd HH: mm: ss
                nodeType = 1,// int 否   节点类型。0：普通节点，1：关键节点
                type = 1,// integer 否   类型， 0:单位工程 1:分部 2:分项 3:检验批
                partitioningId = "221/541/632/898",//   string 否   部位全层级Id，以“/”分隔，例如：221 / 541 / 632 / 898
                partitioningName = "xx楼/xx层/xx段/xx部位名称",//string 否 部位全层级名称，以“/”分隔，必须与部位全层级Id一一对应，例如：xx楼 / xx层 / xx段 /xx部位名称
                completionRate = 0.9,//  double 否   完成率，单位：%
                status = 1,//  integer 是   状态，0:未开始 1:进行中 2:已完成
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.processTaskData");
            Console.WriteLine($"[云筑上传进度]数据发送");
            return content;
        }
        #endregion

        #region 上传工程量汇总

        /// <summary>
        /// 获取上传工程量汇总数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetPartProgressSummaryDataContent()
        {
            var guid = Guid.NewGuid();
            var input = new
            {
                sourceId = guid.ToString(),// string  是   节点Id，数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                projectSysNo = 1,//    int 是   智联项目编号
                workSegmentId = guid.ToString(),//    string  是   工点ID
                workSegmentName = "工点名称",//  string  是   工点名称
                workGroupId = guid.ToString(),// string  是   工区ID
                workGroupName = "工区名称",//  string  是   工区名称
                deviationStatus = 1,// int 是   进度偏差状态枚举。如：1：严重滞后，2：滞后，3：轻微滞后，4：正常
                progressStatus = 1,//  int 是   工程状态 0：未开始1：进行中 2：已完工 3：已停工
                completePercent = 80.3,// decimal 是   进度完成率(保留2位小数) 如：80.32 % 传：80.3
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.partProgressSummaryData");
            Console.WriteLine($"[云筑上传工程量汇总]数据发送");
            return content;
        }
        #endregion

        #region 上传工程量和产值完成情况（按月）

        /// <summary>
        /// 获取上传工程量和产值完成情况Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetPartProgressMonthDataContent()
        {
            var guid = Guid.NewGuid();
            var input = new
            {
                sourceId = guid.ToString(),// string  是   节点Id，数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                projectSysNo = guid.ToString(),//    int 是   智联项目编号
                categoryId = guid.ToString(), //string  是   形象进度项分类ID
                categoryName = "形象进度项分类名称",//    string  是   形象进度项分类名称
                partProgressId = guid.ToString(), //string  是   形象进度项全层级ID
                partProgressName = guid.ToString(), //string  是   形象进度项名称
                planOutPut = 1.0,//decimal 是   计划完成产值。单位：万元(保留2位小数)
                completeOutPut = 1.0,// decimal 是   实际完成产值。单位：万元(保留2位小数)
                unitName = "m",// string  是   设计量计量单位
                designQuantity = 1.0,// decimal 是   设计量, 2位小数 = 1.0,
                planQuantity = 1.0,//    decimal 是   本月计划量, 2位小数
                statisticsMonth = $"{DateTime.Now:yyyy-MM}",//string  是   统计月份。格式：yyyy - MM
                completeQuantity = 1.0,//   decimal 是   本月完成量, 保留2位小数
                totalQuantity = 1.0,//decimal 是   开累完成量，保留2位小数
                totalScale = 1.0,//decimal 是   开累完成比例，保留2位小数
                remainQuantity = 1.0,//decimal 是   剩余量，保留2位小数
                workSegmentId = guid.ToString(),//string  是   工点ID
                workSegmentName = "工点名称",//string  是   工点名称
                workGroupId = guid.ToString(),//string  是   工区ID
                workGroupName = "工区名称",// string  是   工区名称
                needWarning = false,//boolean 否   是否需要预警：false：不预警，true：预警；默认为false
                warningDesc = "预警描述",//string  否   预警描述
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.partProgressMonthData");
            Console.WriteLine($"[云筑上传工程量和产值完成情况]数据发送");
            return content;
        }
        #endregion

        #region 上传工程量和产值完成情况（按天）

        /// <summary>
        /// 获取上传工程量和产值完成情况（按天）Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetPartProgressDayDataContent()
        {
            var guid = Guid.NewGuid();
            var group = new List<object>
            {
                new
                {
                    fileName="文件名称",// string  是   文件名称
                    fileType="0",//     integer 是   文件类型，0:图片 1:视频
                    fileUrl="https://test",//  string 是   文件路径，Url路径
                }
            };
            var input = new
            {
                sourceId = guid.ToString(),// string  是   节点Id，数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                projectSysNo = guid.ToString(),//    int 是   智联项目编号
                categoryId = guid.ToString(), //string  是   形象进度项分类ID
                categoryName = "形象进度项分类名称",//    string  是   形象进度项分类名称
                partProgressId = guid.ToString(), //string  是   形象进度项全层级ID
                partProgressName = guid.ToString(), //string  是   形象进度项名称
                unitName = "m",// string  是   设计量计量单位
                designQuantity = 1.0,// decimal 是   设计量, 2位小数 = 1.0,
                completeDate = $"{DateTime.Now:yyyy-MM}",// string  是   完成日期。格式：yyyy - MM - dd
                completeQuantity = 1.0,//      decimal 是   完成量, 保留2位小数 （按天上报）
                completeOutPut = 1.0,//  decimal 是   实际完成产值。单位：万元(保留2位小数)
                workSegmentId = guid.ToString(),//string  是   工点ID
                workSegmentName = "工点名称",//string  是   工点名称
                workGroupId = guid.ToString(),//string  是   工区ID
                workGroupName = "工区名称",// string  是   工区名称
                constructionDesc = "施工描述",//   string  否   施工描述
                reportName = "上报人员名称",//string  是   上报人员名称
                reportDate = $"{DateTime.Now:yyyy-MM-dd}",//string  是   上报日期 格式：yyyy - MM - dd
                pileStartNo = "开始桩号",//string  否   开始桩号
                pileEndNo = "结束桩号",//string  否   结束桩号
                attachmentList = group,// List    否   附件列表
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.partProgressDayData");
            Console.WriteLine($"[云筑上传工程量和产值完成情况]数据发送");
            return content;
        }
        #endregion

        #region 上传质量

        /// <summary>
        /// 获取上传质量数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetQualityCheckDataContent()
        {
            var guid = Guid.NewGuid();
            var group = new List<object>
            {
                new
                {
                    fileName="文件名称",// string  是   文件名称
                    fileType="0",//     integer 是   文件类型，0:图片 1:视频
                    fileUrl="https://test",//  string 是   文件路径，Url路径
                    businessType ="0",//    integer 是   附件类型, 0:其他 1:检查照片 2:整改照片 3:复查照片
                }
            };
            var input = new
            {
                sourceId = guid.ToString(),// string   是   数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                projectSysNo = guid.ToString(),//      integer 是   项目编号
                checkDate = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//    string  是   检查日期，格式：yyyy - MM - dd HH:mm: ss
                checkUserId = guid.ToString(),//  string 是   检查人Id，多个以“/”分隔，例如：5 / 21 / 33
                checkUserName = "姓名",//    string 是   检查人姓名，多个以“/”分隔，必须与检查人Id一一对应，例如：张三 / 李四 / 王五
                checkUnitId = guid.ToString(),//  string 是   检查单位Id
                checkUnitName = "单位名称",//   string 是   检查单位名称
                checkStatus = 1,//   integer 是   发起单据时，首次检查状态，0:不合格 1:合格
                checkDetail = "问题描述",//  string 否   检查内容 / 问题描述
                subItemId = guid.ToString(),//  string 否   分部分项（检查项）全层级Id，以“/”分隔，例如：142 / 331 / 422
                subItemName = guid.ToString(),//  string 否   分部分项（检查项）全层级名称，以“/”分隔，必须与分部分项（检查项）全层级Id一一对应，例如：地基与基础 / 基础 / 钢筋混凝土扩展基础
                partitioningId = guid.ToString(),//  string 是   检查部位全层级Id，以“/”分隔，例如：221 / 541 / 632 / 898
                partitioningName = guid.ToString(),//      string 是   检查部位全层级名称，以“/”分隔，必须与检查部位全层级Id一一对应，例如：xx楼 / xx层 / xx段 / xx部位名称
                rectifyStatus = 0,//   integer 是 整改状态，0:无需整改 1:待整改 2:待复查 3:复查通过 4:复查未通过
                problemLevel = 1,//     integer 否   不合格时的问题级别，0:轻微问题 1:一般问题 2:严重问题
                rectifyRequire = "整改要求",//   string 否   不合格时的整改要求
                rectifyDateLimit = $"{DateTime.Now.AddDays(20):yyyy-MM-dd HH:mm:ss}",//      string 否   整改时限，格式：yyyy - MM - dd HH: mm: ss
                rectifyUnitId = guid.ToString(),//  string 是   整改单位（或合格时的责任单位）Id
                rectifyUnitName = "整改单位（或合格时的责任单位）名称",//  string 是   整改单位（或合格时的责任单位）名称
                rectifyUserId = guid.ToString(),//  string 否   整改人Id
                rectifyUserName = "整改人名称",//  string 否   整改人名称
                rectifyDescription = "整改描述",//   string 否   整改描述
                rectifyDate = $"{DateTime.Now.AddDays(10):yyyy-MM-dd HH:mm:ss}",//  string 否   整改时间，格式：yyyy - MM - dd HH: mm: ss
                reviewUserId = guid.ToString(),//  string 否   复查人Id
                reviewUserName = "复查人姓名",//  string 否   复查人姓名
                reviewDate = $"{DateTime.Now.AddDays(22):yyyy-MM-dd HH:mm:ss}",//  string 否   复查时间，格式：yyyy - MM - dd HH: mm: ss
                reviewDescription = "复查描述",//  string 否   复查描述
                reviewUnitId = guid.ToString(),//  string 否   复查单位Id
                reviewUnitName = "复查单位名称",//  string 否   复查单位名称
                attachmentList = group,//  array 否   附件列表，详见 附件数据对象格式
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.qualityCheckData");
            Console.WriteLine($"[云筑上传质量]数据发送");
            return content;
        }
        #endregion

        #region 上传安全数据

        /// <summary>
        /// 获取上传安全数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSecurityCheckDataContent()
        {
            var guid = Guid.NewGuid();
            var group = new List<object>
            {
                new
                {
                    fileName="文件名称",// string  是   文件名称
                    fileType="0",//     integer 是   文件类型，0:图片 1:视频
                    fileUrl="https://test",//  string 是   文件路径，Url路径
                    businessType ="0",//    integer 是   附件类型, 0:其他 1:检查照片 2:整改照片 3:复查照片
                }
            };
            var input = new
            {
                sourceId = guid.ToString(),// string   是   数据主键，通过该字段唯一性区分数据，进行新增或更新操作
                projectSysNo = guid.ToString(),//      integer 是   项目编号
                checkDate = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//    string  是   检查日期，格式：yyyy - MM - dd HH:mm: ss
                checkUserId = guid.ToString(),//  string 是   检查人Id，多个以“/”分隔，例如：5 / 21 / 33
                checkUserName = "姓名",//    string 是   检查人姓名，多个以“/”分隔，必须与检查人Id一一对应，例如：张三 / 李四 / 王五
                checkUnitId = guid.ToString(),//  string 是   检查单位Id
                checkUnitName = "单位名称",//   string 是   检查单位名称
                checkStatus = 1,//   integer 是   发起单据时，首次检查状态，0:不合格 1:合格
                checkDetail = "问题描述",//  string 否   检查内容 / 问题描述
                itemClassifactionsId = guid.ToString(),//  string 否   检查项（隐患类型）全层级Id，以“/”分隔
                itemClassifactionsName = guid.ToString(),//  string 否   检查项（隐患类型）全层级名称，以“/”分隔，必须与检查部位全层级Id一一对应
                partitioningId = guid.ToString(),//  string 是   检查部位全层级Id，以“/”分隔，例如：221 / 541 / 632 / 898
                partitioningName = guid.ToString(),//      string 是   检查部位全层级名称，以“/”分隔，必须与检查部位全层级Id一一对应，例如：xx楼 / xx层 / xx段 / xx部位名称
                rectifyStatus = 0,//   integer 是 整改状态，0:无需整改 1:待整改 2:待复查 3:复查通过 4:复查未通过
                problemLevel = 1,//     integer 否   不合格时的问题级别，0:轻微问题 1:一般问题 2:严重问题
                rectifyRequire = "整改要求",//   string 否   不合格时的整改要求
                rectifyDateLimit = $"{DateTime.Now.AddDays(20):yyyy-MM-dd HH:mm:ss}",//      string 否   整改时限，格式：yyyy - MM - dd HH: mm: ss
                rectifyUnitId = guid.ToString(),//  string 是   整改单位（或合格时的责任单位）Id
                rectifyUnitName = "整改单位（或合格时的责任单位）名称",//  string 是   整改单位（或合格时的责任单位）名称
                rectifyUserId = guid.ToString(),//  string 否   整改人Id
                rectifyUserName = "整改人名称",//  string 否   整改人名称
                rectifyDescription = "整改描述",//   string 否   整改描述
                rectifyDate = $"{DateTime.Now.AddDays(10):yyyy-MM-dd HH:mm:ss}",//  string 否   整改时间，格式：yyyy - MM - dd HH: mm: ss
                reviewUserId = guid.ToString(),//  string 否   复查人Id
                reviewUserName = "复查人姓名",//  string 否   复查人姓名
                reviewDate = $"{DateTime.Now.AddDays(22):yyyy-MM-dd HH:mm:ss}",//  string 否   复查时间，格式：yyyy - MM - dd HH: mm: ss
                reviewDescription = "复查描述",//  string 否   复查描述
                reviewUnitId = guid.ToString(),//  string 否   复查单位Id
                reviewUnitName = "复查单位名称",//  string 否   复查单位名称

                attachmentList = group,//  array 否   附件列表，详见 附件数据对象格式
                deleted = false,//  boolean 是   是否删除，true：已删除，false：未删除
            };
            var content = await GetContent(input, "upload.securityCheckData");
            Console.WriteLine($"[云筑上传安全]数据发送");
            return content;
        }
        #endregion

        #region 自动降尘喷淋设备心跳

        /// <summary>
        /// 获取自动降尘喷淋 心跳数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetHeartbeatContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                onLineStatus = 1,//	integer	是	在线状态,1:在线,2:离线
                startStatus = 1,// integer 否   开启状态,1:开启,2:关闭
            };
            var content = await GetContent(input, "upload.dustHeartbeat");
            Console.WriteLine($"[云筑自动降尘喷淋]设备[{input.deviceId}]心跳数据发送");
            return content;
        }
        #endregion

        #region 环境

        /// <summary>
        /// 获取扬尘Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                pm25 = 35.2,//    double 是   PM2.5，单位：μg / m3，支持2位小数
                pm10 = 27.9,//   double 否   PM10，单位：μg / m3，支持2位小数
                tsp = 12.0,//double 否   TSP，单位：μg / m3，支持2位小数
                noise = 63.5,//  double 否   噪声，单位：dB，支持2位小数
                windDirect = 120,//  double 否   风向，正北为0度，正东为90度，正南为180度,正西为270度
                windSpeed = 23.0,//   double 否   风速，单位：m / s，支持2位小数
                temp = 35.0,//   double 否   温度，单位：℃，支持2位小数
                humid = 35.2,//   double 否   湿度，单位：% RH，支持2位小数
                atoms = 12.0,//  double 否   气压，单位：kpa，支持2位小数
            };
            var content = await GetContent(input, "upload.envMonitorLiveData");
            Console.WriteLine($"[云筑环境]设备[{input.deviceId}]数据发送");
            return content;
        }

        #endregion

        #region 塔吊设备参数

        /// <summary>
        /// 获取塔吊Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                longitude = 35.0,//   double  否   设备所在经度
                latitude = 35.0,//double  否   设备所在纬度
                overRide = 35,// integer 是   倍率，单位：倍
                minHeight = 35.0,//   double 否   最小高度，吊钩距离大臂的最小垂直高度，单位：米，支持2位小数
                maxHeight = 35.0,//   double 否   最大高度，吊钩距离大臂的最大垂直高度，单位：米，支持2位小数
                minAmplitude = 35.0,//    double 否   最小幅度，单位：米，支持2位小数
                maxAmplitude = 35.0,//    double 否   最大幅度，单位：米，支持2位小数
                maxFourLiftingCapacity = 35.0,//  double 否	4倍率时最大起重量，单位：吨，支持2位小数
                maxFourLiftingCapacityAmplitude = 35.0,// double 否	4倍率时最大起重量幅度，单位：米，支持2位小数
                maxFourAmplitude = 35.0,//    double 否	4倍率时最大幅度，单位：米，支持2位小数
                maxFourAmplitudeLiftingCapacity = 35.0,// double 否	4倍率时最大幅度起重量，单位：吨，支持2位小数
                maxTwoLiftingCapacity = 35.0,//  double 否	2倍率时最大起重量，单位：吨，支持2位小数
                maxTwoLiftingCapacityAmplitude = 35.0,//  double 否	2倍率时最大起重量幅度，单位：米，支持2位小数
                maxTwoAmplitude = 35.0,// double 否	2倍率时最大幅度，单位：米，支持2位小数
                maxTwoAmplitudeLiftingCapacity = 35.0,//  double 否	2倍率时最大幅度起重量，单位：吨，支持2位小数
                localX = 35.0,//  double 否   防碰撞信息本机X
                localY = 35.0,//  double 否   防碰撞信息本机Y
                liftingArmLength = 35.0,//   double 否   起重臂长，单位：米，支持2位小数
                balanceArmLength = 35.0,//    double 否   平衡臂长，单位：米，支持2位小数
                towerBodyHeight = 35.0,// double 否   塔身高度，单位：米，支持2位小数
                towerHeadHeight = 35.0,// double 否   塔冒高度，单位：米，支持2位小数
            };
            var content = await GetContent(input, "upload.towerCraneDeviceParams");
            Console.WriteLine($"[云筑塔吊]设备[{input.deviceId}]数据发送");
            return content;
        }

        #endregion

        #region 塔吊实时数据

        /// <summary>
        /// 获取塔吊Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneLiveContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//  string  是   设备ID；怎么获取设备ID？
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string  是   设备数据采集时间，时间格式 yyyy - MM - dd HH:mm: ss
                //bit0    风速报警
                //bit1    超重报警
                //bit2    碰撞报警
                //bit3    力矩报警
                //bit4    倾斜报警
                //bit5    前碰撞报警
                //bit6    后碰撞报警
                //bit7    左碰撞报警
                //bit8    右碰撞报警
                //bit9    区域保护前报警
                //bit10   区域保护后报警
                //bit11   区域保护左报警
                //bit12   区域保护右报警
                //bit13   上限位报警
                //bit14   下限位报警
                //bit15   外限位报警
                //bit16   内限位报警
                //bit17   左限位报警
                //bit18   右限位报警
                //bit19   进入区域报警
                //bit20   防碰撞停车角
                //bit21   防碰撞刹车角
                //bit22   保留
                //bit23   保留
                //bit24   保留
                //bit25   保留
                //bit26   保留
                //bit27   保留
                //bit28   保留
                //bit29   保留
                //bit30   保留
                //bit31   保留
                //例如：00000000000000000000010000001100，代表碰撞报警、力矩报警、左碰撞报警。
                warning = "00000000000000000000010000000000",// string 是   报警状态，长度为32位，详见 报警状态说明
                overRide = 35,//  integer 是   倍率，单位：倍
                liftingCapacity = 35.0,//  double 是   起重量，单位：吨，支持2位小数
                safeLiftingCapacity = 35.0,//  double 是   安全起重量，单位：吨，支持2位小数
                ratedMoment = 35.0,//  double 否   额定力矩，支持1位小数
                momentPercent = 35.0,//   double 是   力矩百分比，单位：%，支持2位小数；计算公式：(力矩/额定力矩)*100
                amplitude = 35.0,// double 是   幅度，单位：米，支持2位小数
                round = 35.0,//    double 是   回转，单位：度，支持2位小数
                height = 35.0,//   double 是   高度（起重臂到吊钩的垂直距离），单位：米，支持2位小数
                dipAngleX = 35.0,//    double 是   倾角x，单位：度，支持2位小数
                dipAngleY = 35.0,//    double 是   倾角y，单位：度，支持2位小数
                windSpeed = 35.0,//   double 否   风速，单位：米每秒，支持2位小数
                driverId = "500108199003079558",//    string 否   驾驶员身份证号
                driverName = "姓名",//  string 否   驾驶员姓名
            };
            var content = await GetContent(input, "upload.towerCraneLiveData");
            Console.WriteLine($"[云筑塔吊]设备[{input.deviceId}]监测实时数据发送");
            return content;
        }

        #endregion

        #region 升降机设备参数

        /// <summary>
        /// 获取塔吊Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                longitude = 35.0,//   double  否   设备所在经度
                latitude = 35.0,//double  否   设备所在纬度
                ratedLoad = 35.0,//  double  否   额定载荷，单位：吨，支持2位小数
                earlyWarningCoefficient = 35.0,// double  否   预警系数，单位：%，支持2位小数
                warningCoefficient = 35.0,// double  否   报警系数，单位：%，支持2位小数
                maxFloor = 35,//    integer 否   最大楼层
                maxHeight = 35.0,//  double  否   最大高度, 单位：米
                limitPersonCount = 35,//    integer 否   限载人数
                earlyWarningWindSpeed = 35.0,//   double  否   预警风速，单位：米每秒，支持2位小数
                warningWindSpeed = 35.0,//   double  否   报警风速，单位：米每秒，支持2位小数
            };
            var content = await GetContent(input, "upload.constructionElevatorDeviceParams");
            Console.WriteLine($"[云筑升降机]设备[{input.deviceId}]数据发送");
            return content;
        }

        #endregion

        #region 升降机实时数据

        /// <summary>
        /// 获取塔吊Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterLiveContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//  string  是   设备ID；怎么获取设备ID？
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string  是   设备数据采集时间，时间格式 yyyy - MM - dd HH:mm: ss
                //bit0    风速报警
                //bit1    超重报警
                //bit2    碰撞报警
                //bit3    力矩报警
                //bit4    倾斜报警
                //bit5    前碰撞报警
                //bit6    后碰撞报警
                //bit7    左碰撞报警
                //bit8    右碰撞报警
                //bit9    区域保护前报警
                //bit10   区域保护后报警
                //bit11   区域保护左报警
                //bit12   区域保护右报警
                //bit13   上限位报警
                //bit14   下限位报警
                //bit15   外限位报警
                //bit16   内限位报警
                //bit17   左限位报警
                //bit18   右限位报警
                //bit19   进入区域报警
                //bit20   防碰撞停车角
                //bit21   防碰撞刹车角
                //bit22   保留
                //bit23   保留
                //bit24   保留
                //bit25   保留
                //bit26   保留
                //bit27   保留
                //bit28   保留
                //bit29   保留
                //bit30   保留
                //bit31   保留
                //例如：00000000000000000000010000001100，代表碰撞报警、力矩报警、左碰撞报警。
                warning = "00000000000000000000010000000000",// string 是   报警状态，长度为32位，详见 报警状态说明
                currentLoad = 35.0,//  double  是   载重，单位：吨，支持2位小数
                currentRatedLoad = 35.0,//    double  是   当前额定载荷，单位：吨，支持2位小数
                loadPercent = 35.0,//  double  是   载重百分比，单位：%，支持2位小数
                personCount = 35,//  integer 否   人数
                floor = 35,//   integer 否   楼层
                speed = 35.0,//   double  否   速度，单位：米每秒，支持2位小数
                windSpeed = 35.0,//    double  否   风速，单位：米每秒，支持2位小数
                height = 35.0,//   double  是   高度，单位：米，支持2位小数
                driverId = "500108199003079558",//    string 否   驾驶员身份证号
                driverName = "姓名",//  string 否   驾驶员姓名
                windScale = 10,//   integer 否   风力等级
                dipAngleX = 35.0,//   double  否   倾角x，单位：度，支持2位小数
                dipAngleY = 35.0,//    double  否   倾角y，单位：度，支持2位小数
            };
            var content = await GetContent(input, "upload.constructionElevatorLiveData");
            Console.WriteLine($"[云筑升降机]设备[{input.deviceId}]监测实时数据发送");
            return content;
        }

        #endregion

        #region 卸料平台设备参数

        /// <summary>
        /// 获取卸料平台Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetUnloadingContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                longitude = 35.0,//   double  否   设备所在经度
                latitude = 35.0,//double  否   设备所在纬度
                stdLoadWeight = 35.0,// double  是   标准载重，单位：吨，支持2位小数
                maxDipAngleX = 35.0,// double  否   最大倾角X，单位：度，支持1位小数
                maxDipAngleY = 35.0,// double  否   最大倾角Y，单位：度，支持1位小数
                earlyWarningCoefficient = 35.0,// double  否   预警系数，单位：%，支持2位小数
                warningCoefficient = 35.0,//  double  否   报警系数，单位：%，支持2位小数
                earlyWarningDipAngle = 35.0,//    double  否   倾角预警值，单位：度，支持1位小数
                warningDipAngle = 35.0,// double  否   倾角报警值，单位：度，支持1位小数
            };
            var content = await GetContent(input, "upload.unloadingPlatformDeviceParams");
            Console.WriteLine($"[云筑卸料平台]设备[{input.deviceId}]数据发送");
            return content;
        }

        #endregion

        #region 卸料平台实时数据

        /// <summary>
        /// 获取卸料平台Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetUnloadingLiveContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string  是   设备数据采集时间，时间格式 yyyy - MM - dd HH:mm: ss
                //bit0    重量预警
                //bit1    重量报警
                //bit2    倾斜预警
                //bit3    倾斜报警
                //bit4    保留
                //bit5    保留
                //bit6    保留
                //bit7    保留
                //例如：00001010，代表倾斜报警和重量报警
                warning = "00001010",// string  是   报警状态，长度为8位，详见 报警状态说明
                loadWeight = 35.0,//double  是   载重量，单位：吨，支持2位小数
                dipAngleX = 35.0,//double  否   倾角X，单位：度，支持2位小数
                dipAngleY = 35.0,// double  否   倾角Y，单位：度，支持2位小数
            };
            var content = await GetContent(input, "upload.unloadingPlatformLiveData");
            Console.WriteLine($"[云筑卸料平台]设备[{input.deviceId}]实时数据发送");
            return content;
        }

        #endregion

        #region 吊篮监测设备参数

        /// <summary>
        /// 获取吊篮监测Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetHangingBasketContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备ID
                longitude = 35.0,//   double  否   设备所在经度
                latitude = 35.0,//double  否   设备所在纬度
                stdLoadWeight = 35.0,//  double  是   标准载重，单位：kg，支持2位小数
                stdCounterWeight = 35.0,//   double  否   标准配重，单位：kg，支持2位小数
                errorCounterWeight = 35.0,//  double  否   配重误差，单位：kg，支持2位小数
                maxWaver = 35.0,//   double  否   最大倾斜度，单位：度，支持2位小数
                maxSwingAngle = 35.0,//  double  否   最大摆动度，单位：度，支持2位小数
                maxPersonCount = 35.0,// integer 是   最大人数，单位：位
                minTopHeight = 35.0,//    double  否   离顶端最小距离，单位：米，支持2位小数
            };
            var content = await GetContent(input, "upload.hangingBasketDeviceParams");
            Console.WriteLine($"[云筑吊篮]设备[{input.deviceId}]数据发送");
            return content;
        }

        #endregion

        #region 吊篮实时数据

        /// <summary>
        /// 获取吊篮Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetHangingBasketLiveDataContent()
        {
            var group = new List<object>
            {
                new
                {
                    wireState="01"//string	是	钢丝线状态，长度2位，01:正常 02:异常
                }
            };
            var input = new
            {
                deviceId = "TSVS837819828",//  string  是   设备ID；怎么获取设备ID？
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",// string  是   设备数据采集时间，时间格式 yyyy - MM - dd HH:mm: ss
                currentCounterWeight = 35.0,//   double  是   配重块重量，单位：kg，支持2位小数
                currentLoadWeight = 35.0,//   double  是   载重量，单位：kg，支持2位小数
                waver = 35.0,//   double  是   倾斜，度单位：度，支持2位小数
                swingAngle = 35.0,//  double  是   摆动角度，单位：度，支持2位小数
                currentHeight = 35.0,//   double  是   离顶端距离，单位：米，支持2位小数
                personCount = 35.0,//  integer 是   吊篮上人数，单位：位
                buckleCount = 35.0,//  integer 否   已用卡口数，单位：个
                groupList = group,//  array   否   钢丝线分组
            };
            var content = await GetContent(input, "upload.hangingBasketLiveData");
            Console.WriteLine($"[云筑吊篮]设备[{input.deviceId}]监测实时数据发送");
            return content;
        }

        #endregion

        #region 电表监测 

        /// <summary>
        /// 电表监测ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetElectricityMeterContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                usingCount = 34.0,//  double  是   电表读数，单位：kWh，支持2位小数
            };
            var content = await GetContent(input, "upload.electricityMeterLiveData");
            Console.WriteLine($"[云筑电表监测]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 水表监测 

        /// <summary>
        /// 水表监测ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetWaterMeterContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                usingCount = 34.0,//  double  是   水表读数，单位：m³，支持4位小数
            };
            var content = await GetContent(input, "upload.waterMeterLiveData");
            Console.WriteLine($"[云筑水表监测]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 养护室监测 

        /// <summary>
        /// 养护室监测ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCuringRoomLiveDataContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                temp = 34.0,//double  否   温度，单位：℃，支持2位小数
                humid=10.0,//   double  是   湿度，单位：% RH，支持2位小数
                atoms=12.9//double  否   气压，单位：kpa，支持2位小数
            };
            var content = await GetContent(input, "upload.curingRoomLiveData");
            Console.WriteLine($"[云筑养护室监测]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 烟感监测 

        /// <summary>
        /// 烟感监测ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSmokeDetectorContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                //注：保留位请传0，传值1无效；在没有报警的情况下，请全部传0表示设备在线。
                //位   描述
                //bit0    自检故障
                //bit1    烟雾报警
                //bit2    火警
                //bit3    失联
                //bit4    电量低
                //bit5    传感器故障
                //bit6    震动报警
                //bit7    保留
                //例如：00000010，代表烟雾报警
                warning = "00000000",// string  是   报警状态，长度为8位
                batteryVoltage = 34.0,//	double	否	电池电压
                temp = 34.0,//double  否   温度，单位：℃，支持2位小数
            };
            var content = await GetContent(input, "upload.smokeDetectorLiveData");
            Console.WriteLine($"[云筑烟感监测]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 越界监测 

        /// <summary>
        /// 越界监测ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetIntrusionDetectorContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                //注：保留位请传0，传值1无效。
                //bit0    闯入报警
                //bit1    保留
                //bit2    保留
                //bit3    保留
                //bit4    保留
                //bit5    保留
                //bit6    保留
                //bit7    保留 
                warning = "00000000",// string  是   报警状态，长度为8位，例如：00000001，代表闯入报警
            };
            var content = await GetContent(input, "upload.intrusionDetectorLiveData");
            Console.WriteLine($"[云筑越界监测]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 混凝土测温数据 

        /// <summary>
        /// 混凝土测温数据ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetConcreteContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                temp = 20.1,//double 是	温度，单位：℃，支持2位小数
                stress = 20.1,//   double  是   应力，支持2位小数
            };
            var content = await GetContent(input, "upload.concreteTempLiveData");
            Console.WriteLine($"[云筑混凝土]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 排污监测 

        /// <summary>
        /// 排污监测数据ontent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSewageOutfallContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                temp = 20.1,//double 是	温度，单位：℃，支持2位小数
                phValue = 7.3,// 是	ph值，支持2位小数
            };
            var content = await GetContent(input, "upload.sewageOutfallLiveData");
            Console.WriteLine($"[云筑排污]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 雨水回收 

        /// <summary>
        /// 雨水回收Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetRainRecoveryContent()
        {
            var group = new List<object>
            {
                new
                {
                    groupName ="分组名称",  //string  是   分组名称
                    inTemp  =25.4,  //double  否   室内温度，单位：℃，支持2位小数
                    inHumidity  =10.5  //double  否   室内湿度，单位：%RH，支持2位小数
                }
            };
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                outTemp = 20.1,//double  否   室外温度，单位：℃，支持2位小数
                outHumidity = 10.3,// double  否   室外湿度，单位：% RH，支持2位小数
                waterLevel = 12.2,//  double  否   液位，单位：CM，支持2位小数
                autoWater = 1,//  integer 否   水阀自动补水，0：非自动，1：自动
                timeStart = 1,//  integer 否   定时启用，0：不启用，1：启用
                groupList = group,//  array   否   室内分组列表，详见 室内分组说明
            };
            var content = await GetContent(input, "upload.rainRecoveryLiveData");
            Console.WriteLine($"[云筑雨水回收]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 智慧停车

        /// <summary>
        /// 智慧停车实时监测Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetVehicleManagementContent()
        {
            var input = new
            {
                deviceId = "TSVS837819828",//string  是   设备
                deviceTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//  string  是   设备数据采集时间，时间格式 yyyy-MM-dd HH:mm: ss
                vehicleNumber = "川A12345",//string  是   车牌号：川A12345
                entryOrExit = 0,//integer 是   进出场状态, 0:进场,1:出场
                entryOrExitTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//string 是   进 / 出场时间,格式：yyyy - MM - dd HH: mm: ss
                registeredVehicle = true,//boolean 是   是否登记车辆,true:是，false：否
            };
            var content = await GetContent(input, "upload.vehicleManagementLiveData");
            Console.WriteLine($"[云筑智慧停车]设备[{input.deviceId}]数据发送");
            return content;
        }
        #endregion

        #region 获取content

        private static async Task<HttpContent> GetContent(object input, string api)
        {
            var json = TextJsonConvert.SerializeObject(input).ToLower();
            var guid = Guid.NewGuid().ToString("N");
            var version = $"1.0";
            var now = DateTime.Now;
            var nowStr = $"{now:yyyyMMddHHmmss}";
            var signStr = $"appid=SP.58464811199a4fd78eaeac0afe8273cc&data={json}&format=json&method={api}&nonce={guid}&timestamp={nowStr}&version={version}&appsecret={AppSecret}".ToLower();

            var sign = MD5Encrypt.Encrypt(signStr);//.BuildSign(signStr);

            Console.WriteLine($"******************打印请求参数******************");
            Console.WriteLine($"data:{json}");
            Console.WriteLine($"Json序列化data:{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(json)}");
            Console.WriteLine($"sign:{AppId}");
            Console.WriteLine($"signStr:{signStr}");
            Console.WriteLine($"sign:{sign}");

            var list = new List<KeyValuePair<string, string>>
            {
                //appid   由接口提供方分配给供应商的APP ID。例如：5HGSVWBX8U。
                //format  接口返回结果类型，支持json。
                //method  由接口提供方指定的接口标识符。例如：upload.envMonitorLiveData。详见数据标准的接口描述。
                //nonce   随机数，由调用方生成，防止重复处理和提高验签的安全性的，所以要求要求保证同一timestamp传递不同的nonce值。
                //timestamp   调用方时间戳，格式为“ 4 位年+2 位月+2 位日+2 位小时(24 小时制)+2 位分+2 位秒”，用于接口提供方判断调用方的时间，约定调用请求的时间戳与接口提供方收到请求的时间差在10分钟内。例如：20170215101958。
                //version 由接口提供方指定的接口版本。例如：1.0。详见数据标准的接口描述。
                //data    具体的接口方法中的参数实体信息（对象需要转换成 JSON 格式的字符串）。
                //sign    签名字符串，按照签名生成算法计算得来。签名算法参见签名算法详细说明。
                new KeyValuePair<string, string>("appid", $"{AppId}"),
                new KeyValuePair<string, string>("format","json"),
                new KeyValuePair<string, string>("method",$"{api}"),//api接口
                new KeyValuePair<string, string>("nonce",guid),//随机算法
                new KeyValuePair<string, string>("timestamp",nowStr),
                new KeyValuePair<string, string>("version",$"{version}"),
                new KeyValuePair<string, string>("data",$"{json}"),
                new KeyValuePair<string, string>("sign",$"{sign}")
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            return await Task.FromResult(content);
        }

        #endregion

    }

}
