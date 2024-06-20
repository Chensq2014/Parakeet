using ConsoleApp.Dtos;
using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp.HttpClients;
using Common.Dtos;
using Common.Extensions;

namespace ConsoleApp.Area.Chongqing
{
    /// <summary>
    /// 重庆测试
    /// </summary>
    public static class ChongqingTest
    {
        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "496e092b-17f0-4b67-a961-56abb54fed4c",//"33ce68e6-3b4a-42ee-a8d8-23b818699136",//"b77246d7-b0eb-4a79-91ed-f8c3b6042025",//
            SupplierKeySecret = "bhBDJc448Ul0LS6cGkZPObIDqtQQtjKCLI23",//"2VMq6793Dv0os9p0oYee4LeYSCappXGKH8ez",//"rBPpq1P60jvsWv1cEBVRt2IRj0PCzD4cpEnu", //
            ProjectKeyId = "51f95a92-d9fe-4937-bec2-0cf0926260b5",//"3a19dd66-3346-4d6f-a7f9-3a1af880c07a",//"b547a0e1-8dd5-4733-9b58-8bedeb4e9f2d",//
            ProjectKeySecret = "HKmyboKblHoKE5x5ezmzqPMsYHsedpaCNv8v"//"fPOkwxhT03Z4wNGN8k039DlLjzK1Qdo4JDt0"//"E3mjRc0LsJ7uKxQmIcs4wgDCmnubH3JajMLk"//

            #region 正式环境 重医附一院

            //SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002",
            //SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7",
            //ProjectKeyId = "20c39e24-3c2e-4aea-86b4-cf49f9f7d561",
            //ProjectKeySecret = "6wr87mnaPrdK52pNpWbYpTmzyWPVGzzgi11t"
            #endregion
        };

        /// <summary>
        /// appKey appSecret
        /// </summary>
        private static readonly KeySecret _keySecretV2 = new KeySecret
        {
            #region 测试环境 兰亭景苑地产项目(一期) 500120202012031002

            //SupplierKeyId = "",
            //SupplierKeySecret = "",
            //ProjectKeyId = "a3ff0beb-741d-4599-b3db-f3fad2d87348",
            //ProjectKeySecret = "fa9511e2-a52c-491d-b2a1-751b1e842c1c"
            #endregion

            #region 正式环境 兰亭景苑地产项目(一期) 500120202012031002
            //SupplierKeyId = "",
            //SupplierKeySecret = "",
            //ProjectKeyId = "30606cfe-df24-434e-bf29-dbb1d9f62c1b",
            //ProjectKeySecret = "c3935835-2ca0-4af2-bb99-f4b29243a982"

            #endregion

            #region 重庆东站铁路综合交通枢纽高铁新经济区基础设施工程(开成路、兴塘路拓宽及东延伸段、东侧集散通道) 500108202011131001

            //SupplierKeyId = "",
            //SupplierKeySecret = "",
            //ProjectKeyId = "6fe4c9c9-61e5-423d-b399-0a2f02ca948c",
            //ProjectKeySecret = "c6212696-773d-4587-bad0-724ba341bf7c"
            #endregion


            #region 九滨路与大渡口滨江路连接道工程 500107202010271002

            //SupplierKeyId = "",
            //SupplierKeySecret = "",
            //ProjectKeyId = "f0732c88-f2be-4201-aedb-b22eea1a78eb",
            //ProjectKeySecret = "9d866fa7-307d-442e-83da-39444808b6e6"
            #endregion

            #region 青凤高科创新孵化中心项目(二期)  500106202105251001

            //SupplierKeyId = "",
            //SupplierKeySecret = "",
            //ProjectKeyId = "2b0d9bf2-6ffb-4dab-bb59-19e7e5a451d3",
            //ProjectKeySecret = "cfeffc82-462e-4e49-baf7-abd5beae0511"
            #endregion

            #region 重庆西站铁路综合交通枢纽九龙坡区配套储备土地规划道路工程  500107202107091005

            SupplierKeyId = "",
            SupplierKeySecret = "",
            ProjectKeyId = "e1056277-a979-4ce1-acc9-3ba395bb5f3a",
            ProjectKeySecret = "d4936db3-1aa5-45b7-98b1-25c8a36bd3e7"
            #endregion

        };


        #region 获取token

        /// <summary>
        /// projectCode
        /// //"500106202105251001";//"500107202010271002";//"500108202011131001";//"500120202012031002";
        /// </summary>
        public static string _projectCode = "500107202107091005";

        /// <summary>
        /// Host  测试正式环境 Host均一致
        /// </summary>
        public static string Host = "http://jsgl.zfcxjw.cq.gov.cn";

        /// <summary>
        /// Port   测试：6074  正式：6072
        /// </summary>
        public static string Port = "6074";//"6072";//

        /// <summary>
        /// TokenApi 测试环境："/CSIOTWebService/rest/oauth2/token"  正式环境："/IOTWebService/rest/oauth2/token"
        /// </summary>
        public static string TokenApi = $@"/CSIOTWebService/rest/oauth2/token";//


        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public static async Task<ChongqingTokenResultDto> GetAccessToken(string baseUrl = null, string apiUrl = null)
        {
            try
            {
                //_keySecret.ProjectKeyId = "f80d4bce-403b-44fa-966d-d743412366ff";//appKey
                //_keySecret.ProjectKeySecret = "e3ea8a10-564f-4c19-acb0-8d233ddd4c18";//appSecret
                var tokenResult = await HttpClientPool.GetChongqingAccessToken(_keySecretV2.ProjectKeyId, _keySecretV2.ProjectKeySecret, baseUrl ?? $"{Host}:{Port}", apiUrl ?? TokenApi);
                return tokenResult;
            }
            catch (Exception e)
            {
                Console.WriteLine($"获取token错误：{e.Message}");
                throw;
            }
        }


        #endregion

        #region 项目与项目人员信息

        /// <summary>
        /// 获取项目信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetProjectContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13)
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"获取项目[{input.UnifiedProjectCode}]信息数据发送");
            return content;
        }



        /// <summary>
        /// 获取项目用工接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetProjectWorkerContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,
                page = 1
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"获取项目[{input.UnifiedProjectCode}]用工信息数据发送");
            return content;
        }


        #endregion

        #region 环境

        /// <summary>
        /// 获取扬尘Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetEnvironmentContent()
        {
            var environment = new
            {
                sourceId = "9b2db993-857f-11e7-857d-00163e32d704",
                serialNo = "TSVS837819828",
                temperature = 39.0,
                humidity = 23.0,
                pm2p5 = 35.2,
                pm10 = 27.9,
                noise = 63.5,
                windSpeed = 23.0,
                windDirection = 274,
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mmss")
            };
            var input = new List<object> { environment };

            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆扬尘]设备[{environment.serialNo}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取环境监测实时数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                       //是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),  //是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",                            //是   设备供应商
                DeviceSN = "TSVS837819828",                              //是   设备序列号
                TSP = 39.0.ToString(),                                    //是   TSP（总悬浮颗粒物）//最大10位字符串
                Pressure = 23.0,                                         //是   气压
                WindSpeed = 23.0,                                        //是   风速 
                WindLevel = 3,                                           //是   风级
                Humidity = 35.2,                                         //是   湿度
                Temperature = 35.0,                                      //是   温度
                Noise = 63.5,                                            //是   噪声 
                PM25 = 35.2,                                             //是   PM10
                PM10 = 27.9,                                             //是   PM25
                WindDirection = 274,                                     //是   风向（角度）风向值，整数，范围0°~359°, 单位：°
                                                                         //1:正北方 2：北东北方 3：东北方 4：东东北方 5：正东方 6：东东南方 7：东南方 8：南东南方
                                                                         //9：正南方 10：南西南方 11：西南方 12：西西南方 13：正西方 14：西西北方 15：西北方 16：北西北方
                Direction = 12,                                          //是   风向方位范围：1～16 
                Atmospheric = 71.2,                                      //否   空气质量，精确到小数点后一位
                Date = DateTime.Now.ToString("yyyy-MM-dd"),        //是   采集日期：yyyy - MM - dd
                Time = DateTime.Now.ToString("HH:ss:mm"),          //是 采集时间：HH: ss: mm
                OtherField = "test"                                      //否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };

            var content = new FormUrlEncodedContent(list);

            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆扬尘]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取扬尘定位Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentLocationContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                           //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),            //N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",                                //C20 是   设备供应商
                DeviceSN = "TSVS837819828",                                  //C50 是   设备序列号
                Latitude = 12.2,                                             //N9.6    是   纬度
                Longitude = 71.2,                                            //N9.6    是   经度
                OtherField = "test"//C200    否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆扬尘]设备定位[{input.DeviceSN}]数据发送");
            return content;
        }


        /// <summary>
        /// 获取环境报警Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentWarnContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                              //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),            //N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",                                //C20 是   设备供应商
                DeviceSN = "TSVS837819828",                                  //C50 是   设备序列号
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),             //C10 是   报警开始日期yyyy - MM - dd
                StartTime = DateTime.Now.ToString("HH:ss:mm"),               //C10 是   报警始时间HH:s: mm
                EndDate = DateTime.Now.ToString("yyyy-MM-dd"),               //C10 是 报警结束日期yyyy-MM - dd
                EndTime = DateTime.Now.ToString("HH:ss:mm"),                 //C10 是   报警结束时间HH: ss: mm
                AlarmType = 1.ToString(),                                    //C1  是 报警类型1：PM2.5 2: PM10 3: TSP 4: 风速 5：噪声
                MaxValue = "100",                                            //C10 是 最大报警值
                OtherField = "test"                                             //C200 否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆扬尘][{input.DeviceSN}]报警数据发送");
            return await Task.FromResult(content);
        }

        /// <summary>
        /// 获取环境喷淋Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentSprayContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                      //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),       //N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",                           //C20 是   设备供应商
                DeviceSN = "TSVS837819828",                             //C50 是   设备序列号
                ExceedReason = "超标因素",                              //C200    是   超标因素
                StartDate = DateTime.Now.ToString("yyyy-MM-dd"),        //C10 是   开启日期yyyy - MM - dd
                StartTime = DateTime.Now.ToString("HH:ss:mm"),          //C10 是   开启时间HH:mm: ss
                ExceedData = 123456789,                                 //N10 是 超标数据（ug / m³）
                ResumeData = 123456,                                    //N10 是   恢复数据（ug / m³）
                OpenTime = 300,                                         //N10 是   喷淋（雾炮）打开时长（分钟）
                MaxValue = "100",                                       //N10 是   最大报警值
                OtherField = "test"//保留字段                           //C200    否 扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆扬尘]设备[{input.DeviceSN}]环境喷淋数据发送");
            return await Task.FromResult(content);
        }

        #endregion

        #region 塔吊

        /// <summary>
        /// 获取塔吊Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneContent()
        {
            var input = new List<object>();
            var crane = new
            {
                sourceId = "9b2db993-857f-11e7-857d-00163e32d704",
                serialNo = "渝CS-T00478",
                registNo = "TSVS837819828",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                angle = "105.0",
                radius = 21.0,
                height = 21.0,
                load = 2.1,
                safeLoad = 21,
                percent = 1.61,
                windSpeed = 32,
                obliquity = 0,
                dirAngle = 0,
                fall = 2,
                status = 0,
                message = ""
            };
            input.Add(crane);
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{crane.serialNo}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取塔吊心跳Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneHeartbeatContent()
        {
            var input = new
            {
                registNo = "TSVS837819828",
                serialNo = "渝CS-T00478",
                sendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.serialNo}]心跳数据发送");
            return content;
        }


        /// <summary>
        /// 获取塔吊监测基础数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneBasicContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50必填   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20必填 数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C2必填   设备供应商
                DeviceSN = "TSVS837819828",//C50必填 设备序列号
                SimCardNo = "SIM卡号",//C11必填   SIM卡号
                SelfCraneNo = "99",//N2 必填 塔机名称和编号1~16
                CraneType = 0,//C1 塔机类 0: 动臂吊 1: 塔头平臂吊 2: 平头平臂吊
                WeightSet = 0,//C1 必填 配置载重功能 0:未配置 1:已配置
                WindSpeedSet = 0,//C1 必填 配置风速功能 0:未配置 1:已配置
                RangeSet = 0,//C1 必填 配置幅度功能 0:未配置1: 已配置
                HeightSet = 0,//C1 必填 配置高度功能 0:未配置1: 已配置
                AngleSet = 0,//C1 必填 配置角度功能 0:未配置 1:已配置
                ObliguitySet = 0,//C1 必填 配置倾角功能 0:未配置 1:已配置
                GpsSet = 0,//C1 必填 配GPS功能 0:未配置 1:已配置
                IdSet = 0,//C1 必填 配置人员识别功能 0:未配置 1:已配置
                FrontArmLength = 35.2,//N5.2   必填 前臂长0.00~99.99m
                BackArmLength = 32.1,//N5.2   必填 尾臂长0.0~99.99m
                MaxWeight = 60,//N5.2   必填 最大吊重 000~99.99t
                RatedWindSpeed = 0,//N5.2   必填 额定风速 0.00~36.90m / s
                RatedWindLevel = 8,//N2必填   额定风级 0~12级
                MinRange = 12.3,//N5.2   必填 最小变幅 0.00~99.99m
                MaxRange = 0,//N.2必填 最大变幅 0.00~99.99m
                ArmHeight = 12,//N.2必填 最大臂高 0.00~655.35m
                MaxHeight = 12,//N6.2   必填 最大高度 0.00~655.35m
                MaxAngle = 0,//N6.1   必填 最大角度 540.0°
                MinAngle = 0,//N6.1   必填 最小角度 -540.0°
                RatedObliguity = 0,//N4.2   必填 额定倾角 0.00~9.99°
                TC_MS_LoadCapacity = 0,//N5.2   必填 最大幅度允许最大载重0.00~99.99t
                TC_ML_MaxScope = 0,//N6.2   必填 最大载重达到最大幅度0.00~655.3m
                DifferInput = 0,//N1必填   载重滤波等级  1~9
                SettingMinHeight = 0,//N6.1   必填 高度报警阈值  0.00~655.3m
                WeightZero = 0,//N5.2   必填 载重零点值   0.00~99.99t
                WindSpeedZero = 0,//N5.2   必填 风速零点值   0.00~99.99m / s
                AngleZero = 0,//N5.1   必填 角度零点值   0.0~359.9°
                DipXZero = 0,//N4.2   必填 倾角X零点值  0.00~9.99°
                DipYZero = 0,//N4.2   必填 倾角Y零点值  0.00~9.99°
                ElevationAngleZero = 0,//N5.2   必填 动臂吊仰角零点值    0.00~99.99°
                ForbidEntryFunExist = 0,//C1必填   配置禁入区功能 0:不存在 1：存在
                ForbidEntryExist = 0,//C1 必填 禁入区域是否存在 0:不存在 1：存在
                ForbidEntryAngle0 = 0,//N5.1    否 禁入区域角度10.0~359.9°
                ForbidEntryAngle1 = 0,//N5.1    否 禁入区域角度20.0~359.9°
                ForbidSuspend2FunExist = 0,//C1必填   配置禁吊区A类功能 0:未配置 1：已配置
                ForbidSuspend2Exist0 = 0,//C1 必填 A类1号禁吊区域是否存在 0:不存在 1:存在
                ForbidSuspend2Angle00 = 0,//N5.1   必填 A类1号禁吊区域角度10.0~359.9°
                ForbidSuspend2Angle01 = 0,//N5.1   必填 A类1号禁吊区域角度20.0~359.9°
                ForbidSuspend2Exist1 = 0,//C1必填   A类2号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend2Angle10 = 0,//N5.1   必填 A类2号禁吊区域角度1
                ForbidSuspend2Angle11 = 0,//N5.1   必填 A类2号禁吊区域角度2
                ForbidSuspend2Exist2 = 0,//C1必填   A类3号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend2Angle20 = 0,//N5.1   必填 A类3号禁吊区域角度1
                ForbidSuspend2Angle21 = 0,//N5.1   必填 A类3号禁吊区域角度2
                ForbidSuspend2Exist3 = 0,//C1必填   A类4号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend2Angle30 = 0,//N5.1   必填 A类4号禁吊区域角度1
                ForbidSuspend2Angle31 = 0,//N5.1   必填 A类4号禁吊区域角度2
                ForbidSuspend2Exist4 = 0,//C1必填   A类5号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend2Angle40 = 0,//N5.1   必填 A类5号禁吊区域角度1
                ForbidSuspend2Angle41 = 0,//N5.1   必填 A类5号禁吊区域角度2
                ForbidSuspend4FunExist = 0,//C1必填   配置禁吊区B类功能 0:未配置 1：已配置
                ForbidSuspend4Exist0 = 0,//C1 必填 B类1号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend4Angle00 = 0,//N5.1   必填 B类1号禁吊区域角度10.0~359.9°
                ForbidSuspend4Range00 = 0,//N5.1   必填 B类1号禁吊区域幅度1
                ForbidSuspend4Angle01 = 0,//N5.1   必填 B类1号禁吊区域角度2
                ForbidSuspend4Range01 = 0,//N5.1   必填 B类1号禁吊区域幅度2
                ForbidSuspend4Angle02 = 0,//N5.1   必填 B类1号禁吊区域角度3
                ForbidSuspend4Range02 = 0,//N5.1   必填 B类1号禁吊区域幅度3
                ForbidSuspend4Angle03 = 0,//N5.1   必填 B类1号禁吊区域角度4
                ForbidSuspend4Range03 = 0,//N5.1   必填 B类1号禁吊区域幅度4
                ForbidSuspend4Exist1 = 0,//C1必填   B类2号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend4Angle10 = 0,//N5.1   必填 B类2号禁吊区域角度1
                ForbidSuspend4Range10 = 0,//N5.1   必填 B类2号禁吊区域幅度1
                ForbidSuspend4Angle11 = 0,//N5.1   必填 B类2号禁吊区域角度2
                ForbidSuspend4Range11 = 0,//N5.1   必填 B类2号禁吊区域幅度2
                ForbidSuspend4Angle12 = 0,//N5.1   必填 B类2号禁吊区域角度3
                ForbidSuspend4Range12 = 0,//N5.1   必填 B类2号禁吊区域幅度3
                ForbidSuspend4Angle13 = 0,//N5.1   必填 B类2号禁吊区域角度4
                ForbidSuspend4Range13 = 0,//N5.1   必填 B类1号禁吊区域幅度4
                ForbidSuspend4Exist2 = 0,//C1必填   B类3号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend4Angle20 = 0,//N5.1   必填 B类3号禁吊区域角度1
                ForbidSuspend4Range20 = 0,//N5.1   必填 B类3号禁吊区域幅度1
                ForbidSuspend4Angle21 = 0,//N5.1   必填 B类3号禁吊区域角度2
                ForbidSuspend4Range21 = 0,//N5.1   必填 B类3号禁吊区域幅度2
                ForbidSuspend4Angle22 = 0,//N5.1   必填 B类3号禁吊区域角度3
                ForbidSuspend4Range22 = 0,//N5.1   必填 B类3号禁吊区域幅度3
                ForbidSuspend4Angle23 = 0,//N5.1   必填 B类3号禁吊区域角度4
                ForbidSuspend4Range23 = 0,//N5.1   必填 B类3号禁吊区域幅度4
                ForbidSuspend4Exist3 = 0,//C1必填   B类4号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend4Angle30 = 0,//N5.1   必填 B类4号禁吊区域角度1
                ForbidSuspend4Range30 = 0,//N5.1   必填 B类4号禁吊区域幅度1
                ForbidSuspend4Angle31 = 0,//N5.1   必填 B类4号禁吊区域角度2
                ForbidSuspend4Range31 = 0,//N5.1   必填 B类4号禁吊区域幅度2
                ForbidSuspend4Angle32 = 0,//N5.1   必填 B类4号禁吊区域角度3
                ForbidSuspend4Range32 = 0,//N5.1   必填 B类4号禁吊区域幅度3
                ForbidSuspend4Angle33 = 0,//N5.1   必填 B类4号禁吊区域角度4
                ForbidSuspend4Range33 = 0,//N5.1   必填 B类4号禁吊区域幅度4
                ForbidSuspend4Exist4 = 0,//C1必填   B类5号禁吊区域是否存在 0:不存在 1：存在
                ForbidSuspend4Angle40 = 0,//N5.1   必填 B类5号禁吊区域角度1
                ForbidSuspend4Range40 = 0,//N5.1   必填 B类5号禁吊区域幅度1
                ForbidSuspend4Angle41 = 0,//N5.1   必填 B类5号禁吊区域角度2
                ForbidSuspend4Range41 = 0,//N5.1   必填 B类5号禁吊区域幅度2
                ForbidSuspend4Angle42 = 0,//N5.1   必填 B类5号禁吊区域角度3
                ForbidSuspend4Range42 = 0,//N5.1   必填 B类5号禁吊区域幅度3
                ForbidSuspend4Angle43 = 0,//N5.1   必填 B类5号禁吊区域角度4
                ForbidSuspend4Range43 = 0,//N5.1   必填 B类5号吊区域幅度4
                MultiFunExist = 0,//C1必填   配置多机防撞功能 0:未配置 1:已配置
                MultiNo = 0,//C2 必填 本机的多机防撞组网序号0~15
                MultiChannel = 0,//N2必填   Zigbee多机防撞工作通道11~26
                MultiExist0 = 0,//C1必填   多机防撞0号机存在 0：不存在 1：已存在
                MultiDeviceFactory0 = 0,//C4 必填 多机防撞0号机黑匣子厂家(大写英文字母)
                MultiDeviceSN0 = 0,//C8必填   多机防撞0号机黑匣子编号(8位阿拉伯数字)
                MultiX0 = 0,//N6.2   必填 多机防撞0号机X坐标(0.00~655.3m)
                MultiY0 = 0,//N6.2   必填 多机防撞0号机Y坐标(0.00~655.3m)
                MultiFrontArmLength0 = 0,//N5.2   必填 多机防撞0号机臂长(0.00~99.99m)
                MultiBackArmLength0 = 0,//N5.2   必填 多机防撞0号机尾臂长(0.00~99.99m)
                MultiExist1 = 0,//C1必填   多机防撞1号机存在 0:不存在 1：存在
                MultiDeviceFactory1 = 0,//C4 必填 多机防撞1号机黑匣子厂家
                MultiDeviceSN1 = 0,//C8必填   多机防撞1号机黑匣子编号
                MultiX1 = 0,//N6.2   必填 多机防撞1号机X坐标
                MultiY1 = 0,//N6.2   必填 多机防撞1号机Y坐标
                MultiFrontArmLength1 = 0,//N5.2   必填 多机防撞1号机臂长
                MultiBackArmLength1 = 0,//N5.2   必填 多机防撞1号机尾臂长
                MultiExist2 = 0,//C1必填   多机防撞2号机存在 0:不存在 1：存在
                MultiDeviceFactory2 = 0,//C4 必填 多机防撞2号机黑匣子厂家
                MultiDeviceSN2 = 0,//C8必填   多机防撞2号机黑匣子编号
                MultiX2 = 0,//N6.2   必填 多机防撞2号机X坐标
                MultiY2 = 0,//N6.2   必填 多机防撞2号机Y坐标
                MultiFrontArmLength2 = 0,//N5.2   必填 多机防撞2号机臂长
                MultiBackArmLength2 = 0,//N5.2   必填 多机防撞2号机尾臂长
                MultiExist3 = 0,//C1必填   多机防撞3号机存在 0:不存在 1：存在
                MultiDeviceFactory3 = 0,//C4 必填 多机防撞3号机黑匣子厂家
                MultiDeviceSN3 = 0,//C8必填   多机防撞3号机黑匣子编号
                MultiX3 = 0,//N6.2   必填 多机防撞3号机X坐标
                MultiY3 = 0,//N6.2   必填 多机防撞3号机Y坐标
                MultiFrontArmLength3 = 0,//N5.2   必填 多机防撞3号机臂长
                MultiBackArmLength3 = 0,//N5.2   必填 多机防撞3号机尾臂长
                MultiExist4 = 0,//C1必填   多机防撞4号机存在 0:不存在 1：存在
                MultiDeviceFactory4 = 0,//C4 必填 多机防撞4号机黑匣子厂家
                MultiDeviceSN4 = 0,//C8必填   多机防撞4号机黑匣子编号
                MultiX4 = 0,//N6.2   必填 多机防撞4号机X坐标
                MultiY4 = 0,//N6.2   必填 多机防撞4号机Y坐标
                MultiFrontArmLength4 = 0,//N5.2   必填 多机防撞4号机臂长
                MultiBackArmLength4 = 0,//N5.2   必填 多机防撞4号机尾臂长
                MultiExist5 = 0,//C1必填   多机防撞5号机存在 0:不存在 1：存在
                MultiDeviceFactory5 = 0,//C4 必填 多机防撞5号机黑匣子厂家
                MultiDeviceSN5 = 0,//C8必填   多机防撞5号机黑匣子编号
                MultiX5 = 0,//N6.2   必填 多机防撞5号机X坐标
                MultiY5 = 0,//N6.2   必填 多机防撞5号机Y坐标
                MultiFrontArmLength5 = 0,//N5.2   必填 多机防撞5号机臂长
                MultiBackArmLength5 = 0,//N5.2   必填 多机防撞5号机尾臂长
                MultiExist6 = 0,//C1必填   多机防撞6号机存在 0:不存在 1：存在
                MultiDeviceFactory6 = 0,//C4 必填 多机防撞6号机黑匣子厂家
                MultiDeviceSN6 = 0,//C8必填   多机防撞6号机黑匣子编号
                MultiX6 = 0,//N6.2   必填 多机防撞6号机X坐标
                MultiY6 = 0,//N6.2   必填 多机防撞6号机Y坐标
                MultiFrontArmLength6 = 0,//N5.2   必填 多机防撞6号机臂长
                MultiBackArmLength6 = 0,//N5.2   必填 多机防撞6号机尾臂长
                MultiExist7 = 0,//C1必填   多机防撞7号机存在 0:不存在 1：存在
                MultiDeviceFactory7 = 0,//C4 必填 多机防撞7号机黑匣子厂家
                MultiDeviceSN7 = 0,//C8必填   多机防撞7号机黑匣子编号
                MultiX7 = 0,//N6.2   必填 多机防撞7号机X坐标
                MultiY7 = 0,//N6.2   必填 多机防撞7号机Y坐标
                MultiFrontArmLength7 = 0,//N5.2   必填 多机防撞7号机臂长
                MultiBackArmLength7 = 0,//N5.2   必填 多机防撞7号机尾臂长
                MultiExist8 = 0,//C1必填   多机防撞8号机存在 0:不存在 1：存在
                MultiDeviceFactory8 = 0,//C4 必填 多机防撞8号机黑匣子厂家
                MultiDeviceSN8 = 0,//C8必填   多机防撞8号机黑匣子编号
                MultiX8 = 0,//N6.2   必填 多机防撞8号机X坐标
                MultiY8 = 0,//N6.2   必填 多机防撞8号机Y坐标
                MultiFrontArmLength8 = 0,//N5.2   必填 多机防撞8号机臂长
                MultiBackArmLength8 = 0,//N5.2   必填 多机防撞8号机尾臂长
                MultiExist9 = 0,//C1必填   多机防撞9号机存在 0:不存在 1：存在
                MultiDeviceFactory9 = 0,//C4 必填 多机防撞9号机黑匣子厂家
                MultiDeviceSN9 = 0,//C8必填   多机防撞9号机黑匣子编号
                MultiX9 = 0,//N6.2   必填 多机防撞9号机X坐标
                MultiY9 = 0,//N6.2   必填 多机防撞9号机Y坐标
                MultiFrontArmLength9 = 0,//N5.2   必填 多机防撞9号机臂长
                MultiBackArmLength9 = 0,//N5.2   必填 多机防撞9号机尾臂长
                MultiExist10 = 0,//C1必填   多机防撞10号机存在 0:不存在 1：存在
                MultiDeviceFactory10 = 0,//C4 必填 多机防撞10号机黑匣子厂家
                MultiDeviceSN10 = 0,//C8必填   多机防撞10号机黑匣子编号
                MultiX10 = 0,//N6.2   必填 多机防撞10号机X坐标
                MultiY10 = 0,//N6.2   必填 多机防撞10号机Y坐标
                MultiFrontArmLength10 = 0,//N5.2   必填 多机防撞10号机臂长
                MultiBackArmLength10 = 0,//N5.2   必填 多机防撞10号机尾臂长
                MultiExist11 = 0,//C1必填   多机防撞11号机存在 0:不存在 1：存在
                MultiDeviceFactory11 = 0,//C4 必填 多机防撞11号机黑匣子厂家
                MultiDeviceSN11 = 0,//C8必填   多机防撞11号机黑匣子编号
                MultiX11 = 0,//N6.2   必填 多机防撞11号机X坐标
                MultiY11 = 0,//N6.2   必填 多机防撞11号机Y坐标
                MultiFrontArmLength11 = 0,//N5.2   必填 多机防撞11号机臂长
                MultiBackArmLength11 = 0,//N5.2   必填 多机防撞11号机尾臂长
                MultiExist12 = 0,//C1必填   多机防撞12号机存在 0:不存在 1：存在
                MultiDeviceFactory12 = 0,//C4 必填 多机防撞12号机黑匣子厂家
                MultiDeviceSN12 = 0,//C8必填   多机防撞12号机黑匣子编号
                MultiX12 = 0,//N6.2   必填 多机防撞12号机X坐标
                MultiY12 = 0,//N6.2   必填 多机防撞12号机Y坐标
                MultiFrontArmLength12 = 0,//N5.2   必填 多机防撞12号机臂长
                MultiBackArmLength12 = 0,//N5.2   必填 多机防撞12号机尾臂长
                MultiExist13 = 0,//C1必填   多机防撞13号机存在 0:不存在 1：存在
                MultiDeviceFactory13 = 0,//C4 必填 多机防撞13号机黑匣子厂家
                MultiDeviceSN13 = 0,//C8必填   多机防撞13号机黑匣子编号
                MultiX13 = 0,//N6.2   必填 多机防撞13号机X坐标
                MultiY13 = 0,//N6.2   必填 多机防撞13号机Y坐标
                MultiFrontArmLength13 = 0,//N5.2   必填 多机防撞13号机臂长
                MultiBackArmLength13 = 0,//N5.2   必填 多机防撞13号机尾臂长
                MultiExist14 = 0,//C1必填   多机防撞14号机存在 0:不存在 1：存在
                MultiDeviceFactory14 = 0,//C4 必填 多机防撞14号机黑匣子厂家
                MultiDeviceSN14 = 0,//C8必填   多机防撞14号机黑匣子编号
                MultiX14 = 0,//N6.2   必填 多机防撞14号机X坐标
                MultiY14 = 0,//N6.2   必填 多机防撞14号机Y坐标
                MultiFrontArmLength14 = 0,//N5.2   必填 多机防撞14号机臂长
                MultiBackArmLength14 = 0,//N5.2   必填 多机防撞14号机尾臂长
                MultiExist15 = 0,//C1必填   多机防撞15号机存在 0:不存在 1：存在
                MultiDeviceFactory15 = 0,//C4 必填 多机防撞15号机黑匣子厂家
                MultiDeviceSN15 = 0,//C8必填   多机防撞15号机黑匣子编号
                MultiX15 = 0,//N6.2   必填 多机防撞15号机X坐标
                MultiY15 = 0,//N6.2   必填 多机防撞15号机Y坐标
                MultiFrontArmLength15 = 0,//N5.2   必填 多机防撞15号机臂长
                MultiBackArmLength15 = 0,//N5.2   必填 多机防撞15号机尾臂长
                OtherField = "test"             //C200 否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取塔吊监测实时数据Content  EquipmentRecordNumber?
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneRealTimeContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                                //C50 必填   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),           //N20 必填 数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",                                     //C20 必填   设备供应商
                DeviceSN = "TSVS837819828",                                       //C50 必填 设备序列号
                EquipmentRecordNumber = "设备备案号",                             //C200 必填   设备备案号
                DeviceTCPIP = "127.0.0.1",                                        //C20 必填 黑匣子TCP实际IP
                DeviceTCPPoot = "8090",                                           //C10 必填   黑匣子TCP实际端口
                Date = DateTime.Now.ToString("yyyy-MM-dd"),       //10  必填 采集日期：yyy-MM-dd
                Time = DateTime.Now.ToString("HH:ss:mm"),         //C10 必填   采集时间：HH: ss: mm
                OperaterNo = "操作人员工号",                            //C25 必填 操作人员工号
                Name = "操作人员姓名",                                  //C25 必填   操作人员姓名
                IdNo = "操作人员身份证号",                              //C18 必填 操作人员身份证号
                WorkStatus = 0,                                         //C1 必填   塔机工作状态0: 运行监控 1:顶升监控
                MotorStatus = 0,                                        //C1  必填 系统运行状态  0:无电机处于工作状态 1: 有电机处于工作状态        
                Multiple = 0,                                           //N2  必填 倍率  1~99 
                Moment = 0,                                             //N6.2  必填 力矩百分比   0.00~655.35 %
                Weight = 0,                                             //N5.2  必填 载重  0.00~99.99t
                RatedWeight = 0,                                        //N5.2    必填 额定载重    0.00~99.99t
                WindSpeed = 0,                                          //N5.2    必填 风速  0.00~36.90m / s
                WindLevel = 0,                                          //N2 必填   风级  0~12级
                RRange = 0,                                             //N5.2    必填 幅度  0.00~99.99m
                Height = 0,                                             //N6.2    必填 塔吊高度    0.00~655.35m
                DGHeight = 0,                                           //N6.2    必填 吊钩高度    0.00~655.35m
                Angle = 0,                                              //N5.1    必填 回转角度    0.0~359.9°
                Obliguity = 0,                                          //N4.2    必填 倾角  -9.99~9.99°
                NoError = 0,                                            //C1 必填   无任何外设故障 0:有外设故障 1：无任何故障
                WeightError = 0,                                        //C1  必填 载重传感器故障 0:正常 1：故障
                WindSpeedError = 0,                                     //C1  必填 风速传感器故障 0:正常 1：故障
                RangeError = 0,                                         //C1  必填 幅度传感器故障  0:正常 1：故障
                HeightError = 0,                                        //C1  必填 高度传感器故障 0:正常 1：故障
                AngleError = 0,                                         //C1  必填 角度传感器故障 0:正常 1：故障
                ObliguityError = 0,                                     //C1  必填 倾角传感器故障 0:正常 1：故障
                GpsError = 0,                                           //C1  必填 GPS故障 0:正常 1：故障
                IdError = 0,                                            //C1  必填 身份识别模块故障 0:正常 1：故障
                NoAlarm = 0,                                            //C1  必填 无任何报警 0:有报警 1：无报警
                MomentAlarm = 0,                                        //C1  必填 力矩报警 0:正常 1：报警
                WindSpeedAlarm = 0,                                     //C1  必填 风速报警 0:正常 1：报警
                HeightAlarm = 0,                                        //C1  必填 高度上限位报警 0:正常 1：报警
                MinRangeAlarm = 0,                                      //C1  必填 幅度内限位报警 0:正常 1：报警
                MaxRangeAlarm = 0,                                      //C1  必填 幅度外限位报警 0:正常 1：报警
                PosAngleAlarm = 0,                                      //C1  必填 顺时针转限位报 0:正常 1：报警
                NegAngleAlarm = 0,                                      //C1  必填 逆时针回转限位报警 0:正常 1：报警
                ObliguityAlarm = 0,                                     //C1  必填 倾角报警 0:正常 1：报警
                ForbidEntryAlarm = 0,                                   //C1  必填 禁入区报警 0:正常 1：报警
                ForbidSuspend2Alarm = 0,                                //N1  必填 A类禁吊区域报警0: 正常 1~5:1~5区域报警
                ForbidSuspend4Alarm = 0,                                //N1  必填 B类禁吊区域报警 0: 正常 1~5:~5区域警
                MultiAlarmAll = 0,                                      //C1  必填 多机防撞报警 0:正常 1：报警
                MultiOnline0 = 0,                                       //C1  必填 多机防撞0号机在线 0:离线 1:在线
                MultiRange0 = 0,                                        //N5.2    必填 多机防撞0号机幅度0.00~99.99m
                MultiAngle0 = 0,                                        //N5.1    必填 多机防撞0号机角度0.0~359.9°
                MultiAlarm0 = 0,                                        //C1 必填   多机防撞0号机报警 0: 正常1: 报警
                MultiOnline1 = 0,                                       //C1  必填 多机防撞1号机在线 0:不在线 1：在线
                MultiRange1 = 0,                                        //N5.2    必填 多机防撞1号机幅度
                MultiAngle1 = 0,                                        //N5.1    必填 多机防撞1号机角度
                MultiAlarm1 = 0,                                        //C1 必填   多机防撞1号机报警 0:正常 1：报警
                MultiOnline2 = 0,                                       //C1  必填 多机防撞2号机在线 0:不在线 1：在线
                MultiRange2 = 0,                                        //N5.2    必填 多机防撞2号机幅度
                MultiAngle2 = 0,                                        //N5.1    必填 多机防撞2号机角度
                MultiAlarm2 = 0,                                        //C1 必填   多机防撞2号机报警 0:正常 1：报警
                MultiOnline3 = 0,                                       //C1  必填 多机防撞3号机在线 0:不在线 1：在线
                MultiRange3 = 0,                                        //N5.2    必填 多机防撞3号机幅度
                MultiAngle3 = 0,                                        //N5.1    必填 多机防撞3号机角度 
                MultiAlarm3 = 0,                                        //C1 必填   多机防撞3号机报警 0:正常 1：报警
                MultiOnline4 = 0,                                       //C1  必填 多机防撞4号机在线 0:不在线 1：在线
                MultiRange4 = 0,                                        //N5.2    必填 多机防撞4号机幅度
                MultiAngle4 = 0,                                        //N5.1    必填 多机防撞4号机角度
                MultiAlarm4 = 0,                                        //C1 必填   多机防撞4号机报警 0:正常 1：报警
                MultiOnline5 = 0,                                       //C1  必填 多机防撞5号机在线 0:不在线 1：在线
                MultiRange5 = 0,                                        //N5.2    必填 多机防撞5号机幅度
                MultiAngle5 = 0,                                        //N5.1    必填 多机防撞5号机角度
                MultiAlarm5 = 0,                                        //C1 必填   多机防撞5号机报警 0:正常 1：报警
                MultiOnline6 = 0,                                       //C1  必填 多机防撞6号机在线 0:不在线 1：在线
                MultiRange6 = 0,                                        //N5.2    必填 多机防撞6号机幅度
                MultiAngle6 = 0,                                        //N5.1    必填 多机防撞6号机角度
                MultiAlarm6 = 0,                                        //C1 必填   多机防撞6号机报警 0:正常 1：报警
                MultiOnline7 = 0,                                       //C1  必填 多机防撞7号机在线 0:不在线 1：在线
                MultiRange7 = 0,                                        //N5.2    必填 多机防撞7号机幅度
                MultiAngle7 = 0,                                        //N5.1    必填 多机防撞7号机角度
                MultiAlarm7 = 0,                                        //C1 必填   多机防撞7号机报警 0:正常 1：报警
                MultiOnline8 = 0,                                       //C1  必填 多机防撞8号机在线 0:不在线 1：在线
                MultiRange8 = 0,                                        //N5.2    必填 多机防撞8号机幅度
                MultiAngle8 = 0,                                        //N5.1    必填 多机防撞8号机角度
                MultiAlarm8 = 0,                                        //C1 必填   多机防撞8号机报警 0:正常 1：报警
                MultiOnline9 = 0,                                       //C1  必填 多机防撞9号机在线 0:不在线 1：在线
                MultiRange9 = 0,                                        //N5.2    必填 多机防撞9号机幅度
                MultiAngle9 = 0,                                        //N5.1    必填 多机防撞9号机角度
                MultiAlarm9 = 0,                                        //C1 必填   多机防撞9号机报警 0:正常 1：报警
                MultiOnline10 = 0,                                      //C1  必填 多机防撞10号机在线 0:不在线 1：在线
                MultiRange10 = 0,                                       //N5.2    必填 多机防撞10号机幅度
                MultiAngle10 = 0,                                       //N5.1    必填 多机防撞10号机角度
                MultiAlarm10 = 0,                                       //C1 必填   多机防撞10号机报警 0:正常 1：报警
                MultiOnline11 = 0,                                      //C1  必填 多机防撞11号机在线 0:不在线 1：在线
                MultiRange11 = 0,                                       //N5.2    必填 多机防撞11号机幅度
                MultiAngle11 = 0,                                       //N5.1    必填 多机防撞11号机角度
                MultiAlarm11 = 0,                                       //C1 必填   多机防撞11号机报警 0:正常 1：报警
                MultiOnline12 = 0,                                      //C1  必填 多机防撞12号机在线 0:不在线 1：在线
                MultiRange12 = 0,                                       //N5.2    必填 多机防撞12号机幅度
                MultiAngle12 = 0,                                       //N5.1    必填 多机防撞12号机角度
                MultiAlarm12 = 0,                                       //C1 必填   多机防撞12号机报警 0:正常 1：报警
                MultiOnline13 = 0,                                      //C1  必填 多机防撞13号机在线 0:不在线 1：在线
                MultiRange13 = 0,                                       //N5.2    必填 多机防撞13号机幅度
                MultiAngle13 = 0,                                       //N5.1    必填 多机防撞13号机角度
                MultiAlarm13 = 0,                                       //C1 必填   多机防撞13号机报警 0:正常 1：报警
                MultiOnline14 = 0,                                      //C1  必填 多机防撞14号机在线 0:不在线 1：在线
                MultiRange14 = 0,                                       //N5.2    必填 多机防撞14号机幅度
                MultiAngle14 = 0,                                       //N5.1    必填 多机防撞14号机角度
                MultiAlarm14 = 0,                                       //C1 必填   多机防撞14号机报警 0:正常 1：报警
                MultiOnline15 = 0,                                      //C1  必填 多机防撞15号机在线 0:不在线 1：在线
                MultiRange15 = 0,                                       //N5.2    必填 多机防撞15号机幅度
                MultiAngle15 = 0,                                       //N5.1    必填 多机防撞15号机角度
                MultiAlarm15 = 0,                                       //C1 必填   多机防撞15号机报警 0:正常 1：报警
                OtherField = "test"                                   //C200    否 保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.DeviceSN}]监测实时数据发送");
            return content;
        }


        /// <summary>
        /// 获取塔吊监测工作循环数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneWorkCycleContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50必填   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20必填 数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C20必填   设备供应商
                DeviceSN = "TSVS837819828",//C50必填 设备序列号
                OperaterNo = "操作人员工号",//C25必填   操作人员工号
                Name = "操作人员姓名",// C25必填 操作人员姓名
                IdNo = "操作人员身份证号",//C18必填   操作人员身份证号
                WorkStartDate = DateTime.Now.ToString("yyyy-MM-dd"),//C10必填 工作循环开始日期yyyy-MM - dd
                WorkStartTime = DateTime.Now.ToString("HH:ss:mm"),//C10必填   工作循环开始时间HH: ss: mm
                WorkEndDate = DateTime.Now.ToString("yyyy-MM-dd"),//C10必填 工作循环结束日期yyyy-MM - dd
                WorkEndTime = DateTime.Now.ToString("HH:ss:mm"),//C10必填   工作循环结束时间HH: ss: mm
                WorkWeight = 0,//N5.2   必填 工作循环最大力矩时的吊重    0.00~99.99t
                WorkMaxTorqueRange = 0,//N5.2   必填 工作循环最大力矩时的幅度    0.00~99.99
                WorkRatedWeight = 0,//N5.2   必填 工作循环最大力矩时的额定载重  0.00~99.99t
                WorkMaxTorque = 0,//N6.2   必填 工作循环最大力矩百分比 0.00~655.35 %
                WorkMaxWindSpeed = 0,//N5.2   必填 工作循环最大风速    0.00~36.90m / s
                WorkWindSpeedAlarm = 0,//C1必填   工作循环中是否有风速报警    0:正常 1:报警 2:预警
                WorkMaxHeight = 0,//C1 必填 工作循环中高度变化量 0.00~655.35m
                WorkMaxForce = 0,//C1必填   工作循环中最大力矩 0.00~999.99t·m
                WorkStartAngle = 0,//C1 必填 工作循环开始角度 0.0~359.9°
                WorkEndAngle = 0,//C1必填   工作循环结束角度 0.0~359.9°
                WorkStartRange = 0,//C1必填   工作循环开始幅度 0.00~99.99m
                WorkEndRange = 0,//C1必填   工作循环结束幅度 0.00~99.99m
                WorkStartHeight = 0,//C1必填   工作循环开始高度 0.00~655.35m
                WorkEndHeight = 0,//C1必填   工作循环结束高度 0.00~655.35m
                WorkMaxRangeAlarm = 0,//C1必填   工作循环中是否出现幅度外限位报警 0:正常  1：报警
                WorkMinRangeAlarm = 0,//C1 必填 工作循环中是否出现幅度内限位报警 0:正常  1：报警
                WorkHeightAlarm = 0,//C1 必填 工作循环中是否出现高度上限位报警 0:正常  1：报警
                WorkPosAngleAlarm = 0,//C1 必填 工作循环中是否出现顺时针回转限位报警 0:正常  1：报警
                WorkNegAngleAlarm = 0,//C1 必填 工作循环中是否出现逆时针回转限位报警 0:正常  1：报警
                WorkMomentAlarm = 0,//C1 必填 工作循环中是否出现力矩报警 0:正常  1：报警
                WorkObliguityAlarm = 0,//C1 必填 工作循环中是否出现倾角报警 0:正常  1：报警
                WorkEnvironmentAlarm = 0,//C1 必填 工作循环中是否出现环境防碰撞报警 0:正常  1：报警
                WorkMultiAlarm = 0,//C1 必填 工作循环中是否出现多机防碰撞报警 0:正常  1：报警
                WorkMomentPreAlarm = 0,//C1 必填 工作循环中是否出现力矩预警 0:正常  1：报警
                WorkWindSpeedPreAlarm = 0,//C1 必填 工作循环中是否有风速报警 0:正常  1：报警
                StartAngle = 0,//C10必填 开始仰角(动臂) 单位: 0.1°  10 进制
                EndAngle = 0,//C10必填 结束仰角(动臂) 单位: 0.1°  10 进制
                OtherField = "test"//C200    否 保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.DeviceSN}]数据发送");
            return content;
        }


        /// <summary>
        /// 获取塔吊监测定位数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneLocationContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50必填   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20必填   数据时间戳(示例精确到毫秒：191231369050)
                DeviceFactory = "设备供应商",//C20必填   设备供应商
                DeviceSN = "TSVS837819828",//C50必填   设备序列号
                Latitude = 12.2,//N9.6   必填   纬度
                Longitude = 71.2,//N9.6   必填   经度
                OtherField = "test"//C200    否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.DeviceSN}]数据发送");
            return content;
        }


        /// <summary>
        /// 获取塔吊监攀爬防护Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetClimbingTowerCraneContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 必填   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20 必填   数据时间戳(示例精确到毫秒：159123136050)
                DeviceFactory = "设备供应商",//C50 必填   设备厂商
                DeviceSN = "TSVS837819828",//C50 必填   设备序列号
                driverId = "操作员身份证号",//C50 必填   操作员身份证号
                driverLicence = "操作证书编号",//C50 必填   操作证书编号
                driverName = "操作员姓名",//C50 必填   操作员姓名
                acpower = 220,//C50 必填   交流电源电压，单位 V
                acalarm = 0,//C1  必填   交流电状态   0:正常 1:警告
                dcpower = 12,//C50 必填 直流电源电压，单位 V
                dcalarm = 0,//C1 必填   直流电状态   0:正常 1:警告
                remainpower = "74.5%",//C50 必填 剩余电量，百分比(74.5意思必填74.5 %)
                state = 0,//C1 必填   设备状态    0:空载 1:正常载人 2:过载
                speed = 0,//C50 必填 速度，单位 m/ s
                pull = 0,//C50 必填   钢绳对人拉力（攀爬助力）
                height = 0,//C50 必填   攀爬高度
                dayload = 0,//C50 必填   当天负载运行次数
                dayoverload = 0,//C50 必填   当天过载次数
                daycharge = 0,//C50 必填   当天充电次数
                daybatalarm = 0,//C50 必填   当天电池告警次数
                Latitude = 0,//N4.2    必填   经度
                Longitude = 0,//N4.2    必填   纬度
                recordTime = 0,//C10 必填   采集时间(yyyy - MM - dd HH: mm:ss)
                speel = 0,//C1 必填   在离岗标识   0:开始 1:结束
                OtherField = "test"//C200    必填 扩展字段    OtherField
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆塔吊]设备[{input.DeviceSN}]数据发送");
            return content;
        }


        #endregion

        #region 升降机


        /// <summary>
        /// 获取升降机Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterContent()
        {
            var input = new List<object>();
            var lifter = new
            {
                sourceId = "9b2db993-857f-11e7-857d-00163e32d704",
                serialNo = "渝ZZ-S00125",
                registNo = "TSVS837819828",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                stopTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                startHeight = 12,
                stopHeight = 15,
                speed = 2.1,
                load = 13,
                loadPercent = 80,
                direction = 1,
                driverId = "",
                driverLicence = "",
                driverName = "",
                status = 0,
                message = ""
            };
            input.Add(lifter);
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{lifter.serialNo}]数据发送");
            return content;
        }


        /// <summary>
        /// 获取升降机心跳Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterHeartbeatContent()
        {
            var input = new
            {
                registNo = "TSVS837819828",
                serialNo = "渝CS-T00478",
                sendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{input.serialNo}]心跳数据发送");
            return content;
        }


        /// <summary>
        /// 获取升降机脚手架基础信息Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterSmartClimbContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C50 是   设备序列号
                DeviceSN = "TSVS837819828",//C50 是   设备厂商
                driverId = "操作员身份证号",//C50 是   操作员身份证号
                driverLicence = "操作证书编号",//C50 否   操作员证书编号
                driverName = "操作员姓名",//C50     操作员姓名
                weight = 0,//N.2 是   重量
                startTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//C20 是   开始工作时间(yyyy - MM - dd HH: mm:ss)
                stopTime = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",//C20 是   停止工作时间(yyyy - MM - dd HH: mm:ss)
                focus = new string[0, 0, 0, 0],//C20 是   各着力点受力情况 单位N   注意是数组
                Latitude = 0,//N18.2   是 经度
                Longitude = 0,//N18.2   是 纬度
                status = 0,//C1 是   状态  0:正常 1:失载 2:超载 3:失载预警 4:超载预警
                message = 0,//C50 是 预警/ 报警 / 违章消息，非正常状态时必须传入
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),//C50 是 采集时间(yyyy-MM - dd HH: mm: ss)
                OtherField = "test"//C200 否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{input.DeviceSN}]脚手架基础信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取升降机监测基础信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterBasicContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//    N20 是 数据时间戳(示例确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C10 是   设备供应商
                DeviceSN = "TSVS837819828",//    C50 是 设备序列号
                SimCardNo = "SIM卡号",//C11 是   SIM卡号
                SelfCraneNo = "名称和编号",
                CraneType = 0,            //    C1  是 升机类型    1: 单笼 2: 双笼
                IdExist = 0,              //    C1  是 配置工号识别功能    0:未配置 1:已配置
                PeopleExist = 0,          //    C1  是 配置人数检测功能    0:未配置 1:已配置
                WeightExist = 0,          //    C1  是 配置载重功能  0:未配置 1:已配置
                SpeedExist = 0,           //    C1  是 配置速度检测功能    0未配置 1:已配置
                HeightExist = 0,          //    C1  是 配置高度检测功能    0:未配置 1:已配置
                FloorExist = 0,           //    C1  是 配置楼层功能  0:未配置1: 已配置
                ObliguityXExist = 0,      //    C1  是 配置X轴倾角功能    0:未配置 1:已配置
                ObliguityYExist = 0,      //    C1  是 配置Y轴倾角能 0:未配置 1:已配置
                WindExist = 0,            //    C1  是 配置风速功能  0:未配置 1:已配置
                GpsExist = 0,             //    C1  是 配置GPS定位功能   0:未配置 1:已配置
                WirelessExist = 0,        //    C1  是 配置无线功能  0:未配置 1:已配置
                TopExist = 0,             //    C1  是 配置上下限位(冲顶)检测功能  0:未配置 1:已配置
                AntiDropExist = 0,        //    C1  是 配置防坠器在位检测功能 0:未配置 1:已配置
                OutDoorStatus = 0,        //    C1  是 外笼门状态   0:打开 1:关闭
                RatedPeople = 0,          //    N2  是 额定运载人数  1 - 20人
                RatedWeight = 0,          //    N4.2    是 额定载重    0.00~9.99t
                RatedUpSpeed = 0,         //    N4.2    是 额定上行速度  0.00~9.99m / s
                RatedDownSpeed = 0,       //N4.2    是 额定下行速度  0.00~9.99m / s
                RatedHeight = 0,          //N5.1    是 额定高度    0.0~999.9m
                MaxFloor = 0,             //N3 是   最大楼层    0~100层
                MinFloor = 0,           //    N2  是 最小楼层    0~-3层
                RatedObliguityX = 0,//N4.2    是 额定X轴倾角  0.00~9.99°
                RatedObliguityY = 0,//N4.2    是 额定Y轴倾角  0.00~9.99°
                RatedWindSpeed = 0,//N5.2    是 额定风速    0.00~36.90m / s
                RatedWindLevel = 0,//   N2 是   额定风级    0~12级
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OtherField = "test" //C200    否 保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆]设备[{input.DeviceSN}]获取升降机监测基础信息数据发送");
            return await Task.FromResult(content);
        }


        /// <summary>
        /// 获取升降机监测实时数据接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterRealTimeContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C20 是   设备供应商
                DeviceSN = "TSVS837819828",//C50 是   设备序列号
                EquipmentRecordNumber = "EquipmentRecordNumber", //C200    是   设备备案号
                DeviceTCPIP = "127.0.0.1",//C20 是   黑匣子TCP实际IP
                DeviceTCPPoot = "8090",//C10 是   黑匣子TCP实际端口
                Date = DateTime.Now.ToString("yyyy-MM-dd"),//C10 是   采集日期：yyyy - MM - dd
                Time = DateTime.Now.ToString("HH:ss:mm"),//C10 是   采集时间：HH:ss: mm
                OperaterNo = "操作人员工号",//C25 是 操作人员工号
                Name = "操作人员姓名",//C25 是   操作人员姓名
                IdNo = "操作人员身份证号",//C18 是 操作人员身份证号
                RealtimePeople = 0,//N2 是   实时运载人数
                RealtimeWeight = 0,//N4.2    是 实时载重
                RealtimeSpeed = 0,//N4.2    是 实时运行速度
                RealtimeHeight = 0,//N5.1    是 实时高度
                RealtimeFloor = 0,//N3 是   实时楼层
                RealtimeDipX = 0,//N4.2    是 实时X倾角
                RealtimeDipY = 0,//N4.2    是 实时Y倾角
                RealtimeWindSpeed = 0,//N5.2    是 实时风速
                RealtimeWindLevel = 0,//C2 是   实时风级
                WorkingType = 0,//C2  是 运行状态
                NoError = 0,//C1 是   无任何外设故障
                IdError = 0,//C1  是 身份识别感器故障
                PeopleCntSetError = 0,//C1 是   运人数传感器故障
                WeightError = 0,//C1  是 载重传感故障
                SpeedError = 0,//C1 是   运行速传感器故障
                HeightError = 0,//C1  是 高度传感故障
                FloorSetError = 0,//C1 是   楼层传感器故障
                ObliguityXError = 0,//C1  是 X轴倾角传感器故障
                ObliguityYError = 0,//C1 是   X轴倾角感器故障
                WindSpeedSetError = 0,//C1  是 风速传感器故障
                GpsError = 0,//C1 是   GPS位模块故障
                WirelessSetError = 0,//C1  是 无模块故障
                NoPreAlarm = 0,//C1 是   无任何预警
                WeightPreAlarm = 0,//C1  是 载重预警
                SpeedPreAlarm = 0,//C1 是   运行速度预警
                HeightPreAlarm = 0,//C1  是 高度预警
                ObliguityXPreAlarm = 0,//C1 是   X轴倾角预警
                ObliguityYPreAlarm = 0,//C1  是 Y轴倾角预警
                WindSpeedPreAlarm = 0,//C1 是   风速预警
                NoAlarm = 0,//C1  是 无任何报警
                PeopleAlarm = 0,//C1 是   运载人数报警
                WeightAlarm = 0,//C1  是 载重报警
                SpeedAlarm = 0,//C1 是   运行速度报警
                HeightAlarm = 0,//C1  是 高度报警
                ObliguityXAlarm = 0,//C1 是   X轴倾角报警
                ObliguityYAlarm = 0,//C1  是 Y轴倾角报警
                WindSpeedAlarm = 0,//C1 是   风速报警
                LimitAlarm = 0,//C1  是 上下限位报警
                AntiDropAlarm = 0,//C1 是   防坠器检测不在位报警
                OutDoorStatus = 0,//C1  是 笼门状态
                OtherField = "test"//C200 否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{input.DeviceSN}]监测实时数据数据发送");
            return content;
        }

        /// <summary>
        /// 获取升降机监测工作循环数据接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterWorkCycleContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C20 是   设备供应商
                DeviceSN = "TSVS837819828",//C50 是   设备序列号
                Name = "操作人员姓名",//C25 是   操作人员姓名UTF - 8编码规则
                IdNo = "操作人员身份证号",//C18 是   身份证号18位
                WorkStartDate = 0,//C10 是   工作循环开始日期yyyy - MM - dd
                WorkStartTime = 0,//C10 是   工作循环开始时间HH:ss: mm
                WorkEndDate = 0,//C10 是 工作循环结束日期yyyy-MM - dd
                WorkEndTime = 0,//C10 是   工作循环结束时间HH: ss: mm
                MaxPeople = 0,//N2  是 最大运载人数1~20人
                MaxWeight = 0,//N4.2    是 最大载重0.00~9.99t
                MaxSpeed = 0,//N4.2    是 最大运行速度0.00~9.99m / s
                StartHeight = 0,//N5.2    是 开始运行高度0.0~999.9m
                EndHeight = 0,//N5.2    是 结束运行高度0.0~999.m
                StartFloor = 0,//N3 是   开始运行楼层0~100层
                EndFloor = 0,//N3  是 结束运行楼层0~100层
                MaxDipX = 0,//N4.2    是 最大x轴倾角0.0~2.00°
                MaxDipY = 0,//N4.2    是 最大y轴倾角0.0~2.00°
                MaxWindSpeed = 0,//N5.2    是 最大风速0.00~36.90m / s
                MaxWindLevel = 0,//N2 最大风级0~12级
                NoAlarm = 0,//C1  是 无任何报警   0:报警 1:无任何报警
                PeopleAlarm = 0,//C1  是 运载人数报警  0:正常 1:报警
                WeightAlarm = 0,//C1  是 载重报警    0: 正常 1:报警
                SpeedAlarm = 0,//C1  是 运行速度报警  0: 正常 1:报警
                HeightAlarm = 0,//C1  是 高度报警    0: 正常 1:报警
                ObliguityXAlarm = 0,//C1  是 X轴倾角报警  0: 正常 1:报警
                ObliguityYAlarm = 0,//C1  是 Y轴倾角报警  0: 正常 1:报警
                WindSpeedAlarm = 0,//C1  是 风速报警    0: 正常 1:报警
                LimitAlarm = 0,//C1  是 上下限位报警  0: 正常 1:上限位报警 2:下限位报警
                AntiDropAlarm = 0,//C1  是 防坠器检测不在位报警  0: 正常 1:报警
                OtherField = "test"//C200    否 保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        /// <summary>
        /// 获取升降机监测定位数据接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetLifterLocationContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//    N20 是 数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商",//C20 是   设备供应商
                DeviceSN = "TSVS837819828",//    C50 是 设备序列号
                Latitude = 12.2,        //N9.6    是 纬度
                Longitude = 71.2,       //N9.6    是 经度
                OtherField = "test"     //C200 否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆升降机]设备[{input.DeviceSN}]数据发送");
            return content;
        }


        #endregion

        #region 设备操作人员


        /// <summary>
        /// 获取设备操作人员身份识别接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetOperatorIdentityContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N0  是   数据间戳(示例精确毫秒：15923136950)
                DeviceFactory = "设备供应商",//C20 是   设备供应商
                DeviceSN = "TSVS837819828",//C50 是   设备序列号
                DeviceType = "01",//C50 是   01 = 塔吊 02 = 升降机
                IdCardNumber = "02",//C18 是   身份证号
                IdCardInfo = 0,//N2  是   身份信息 0 = 未知人 1 = 塔机司机 2 = 检测人员 3 = 安拆人员 4 = 维保人员 5 = 塔机信号工 6 = 监理人员 7 = 项目经理 8 = 安全员 9 = 其他预留
                OtherField = "test"//C200    否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆]设备[{input.DeviceSN}]操作人员身份识别数据发送");
            return content;
        }


        #endregion

        #region 卸料平台


        /// <summary>
        /// 获取卸料平台监测基础信息接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetUnloadingPlatformContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode, //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13), //N20 是   数据时间戳(示例精确到毫秒：191231369050)
                DeviceFactory = "设备供应商", //C20 是   设备供应商
                DeviceSN = "TSVS837819828", //C50 是   设备序列号
                state = 0, //C1  是   卸料平台类型0：正常，1：超重(若不传相关参数，则系统会默认为0)
                load = 0, //C10 是   载重预警阈值
                remainBattery = 0, //C10 是   电池电量预警阈值 %
                loadWarn = 0, //C10 是   载重报警阈值
                remainBatteryWarn = 0, //C10 是   电池电量报警阈值 %
                dipXWarn = 0, //N4.2    否   倾角X预警阈值
                dipYWarn = 0, //N4.2    否   倾角Y预警阈值
                dipXAlarm = 0, //N4.2    否   倾角X报警阈值
                dipYAlarm = 0, //N4.2    否   倾角Y报警阈值
                OtherField = "test" //C200    否   保留字段

            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆卸料平台]设备[{input.DeviceSN}]监测基础信息数据发送");
            return content;
        }

        /// <summary>
        /// 获取卸料平台监测实时数据接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetUnloadingPlatformRealTimeContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode, //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13), //N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商", //C20 是   设备供应商
                DeviceSN = "TSVS837819828", //C50 是   设备序列号
                Date = DateTime.Now.ToString("yyyy-MM-dd"), //C10 是   采集日期
                Time = DateTime.Now.ToString("HH:ss:mm"), //C10 是   采集时间
                Load = 0, //C10 是   载重
                LoadPercent = 0, //C10 是   载重百分比
                BatteryPercent = 0, //N3  是   剩余电量百分比（0 - 100）
                DipX = 0, //N4.2    否   倾角X(-9.99°~9.99°)
                DipY = 0, //N4.2    否   倾角Y(-9.99°~9.99°)
                LoadStatus = 0, //C1  是   载重状态（0：正常；1：预警；2：报警；3：故障）
                BatteryStatus = 0, //C1  是   电量状态（0：正常；1：预警；2：报警；3：故障）
                DipXStatus = 0, //C1  否   倾角X状态（0：正常；1：预警；2：报警；3：故障）
                DipYStatus = 0, //C1  否   倾角Y状态（0：正常；1：预警；2：报警；3：故障）
                WorkStatus = 0, //C1  是   工作状态（0空闲 ；1工作）
                WireTension = 0, //C10 是   钢丝拉力
                WireCode = 0, //C10 是   钢丝编号
                WireType = 0, //C1  是   钢丝类型（1为主缆，0为副钢缆）
                OtherField = "test" //C200    否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆卸料平台]设备[{input.DeviceSN}]监测实时数据数据发送");
            return content;
        }


        #endregion

        #region 视频

        /// <summary>
        /// 获取视频Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetVideoContent()
        {
            _keySecret.SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002";
            _keySecret.SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7";
            _keySecret.ProjectKeyId = "20c39e24-3c2e-4aea-86b4-cf49f9f7d561";
            _keySecret.ProjectKeySecret = "6wr87mnaPrdK52pNpWbYpTmzyWPVGzzgi11t";
            var input = new List<object>();
            var video = new
            {
                //sourceId = "9b2db993-857f-11e7-857d-00163e32d704",
                //serialNo = "渝ZZ-S00125",
                //showName = "test",
                //camNo = "123465",
                //coverUrl = "http://9b2db993-857f-11e7-857d-00163e32d704",
                //getUrl = "http://9b2db993-857f-11e7-857d-00163e32d704",
                //playUrl = "http://9b2db993-857f-11e7-857d-00163e32d704"
                sourceId = "ae23d7df-13e1-b879-0e59-39f716fa5456",
                serialNo = "50010310020001",
                showName = "test",
                camNo = "123465",
                coverUrl = "https://www.spdspd.com/images/none.png",
                getUrl = "http://hls01open.ys7.com/openlive/5435608fa53f4487bb75f711306fe37d.hd.m3u8",
                playUrl = "http://hls01open.ys7.com/openlive/5435608fa53f4487bb75f711306fe37d.hd.m3u8"
            };
            input.Add(video);
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆视频]设备[{video.serialNo}]数据发送");
            return content;
        }



        /// <summary>
        /// 获取视频监控数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetVideoSurveillanceContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,                      //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),       //N20 是   数据时间戳
                DeviceFactory = "设备供应商",                           //C50 是   设备厂商
                DeviceSN = "TSVS837819828",                             //C50 是   设备序列号
                serialType = "视频点属性",                              //C100    是   视频点属性(主要作业面、料场、材料加工区、仓库、围墙、塔吊)
                serialNo = "摄像头物理编号",                            //C100    是   摄像头物理编号
                showName = "监控区域名称",                              //C100    否   监控区域名称例如“大门”，用于平台界面显示, 非必项
                camNo = "摄像头编号",                                   //C100    是   摄像头编号
                coverUrl = "视频的封面图片",                            //C100    是   视频的封面图片
                getUrl = "获取可播放的视频流的url地址",                 //C100    与playUrl不能同时为空  获取可播放的视频流的url地址，需要包含查询参数，监管平台会访问此地址获取视频
                playUrl = "可直接播放的视频流地址",                     //C100    与getUrl不能同时为空   可直接播放的视频流地址，如果该项有值平台会直接取该地址进行播放
                openType = "视频开启状态",                              //C10 是   视频开启状态
                pushTime = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm"),   //C100    是   视频上传时间
                playBackUrl = "视频回放地址",                              //C100    否   视频回放地址
                operateUrl = "视频云台操作地址",                           //10  否   视频云台操作地址
                OtherField = "test"                                //C200    否   保留字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆考勤]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        #endregion

        #region 考勤闸机

        /// <summary>
        /// 获取考勤ontent
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateContent()
        {
            var input = new List<object>();
            var gate = new
            {
                workerId = "013b0bcc-5d8a-4365-a3bb-b9eb8cd59ea0",
                sn = "seialNo",
                entry = "Entry",
                mode = "IDCard",
                photo = "http://013b0bcc-5d8a-4365-a3bb-b9eb8cd59ea0",
                attendanceTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                lat = 1,
                lng = 1
            };
            input.Add(gate);
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆考勤]设备[{gate.sn}]数据发送");
            return content;
        }



        /// <summary>
        /// 获取智慧工地实名制接口Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetGateContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode,//C50 是   工程项目编码
                IdCardNumber = "23432423424",//C18 是   证件号码（证件号和人员ID不能同时为空）
                WorkerId = "WorkerId",//C50 是   人员ID（证件号和人员ID不能同时为空，且不能为0）
                DeviceFactory = "设备供应商",//C20 是   设备供应商
                AttendanceDate = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm"),//C50 是   考勤时间(格式为 yyyy - MM - dd HH: mm:ss）
                Direction = 0,//C1  是 进出方向(1:进场，0：出场)
                Image = Utilities.Base64Phto2,//C50 是   考勤图片URL
                DeviceSN = "TSVS837819828",//C20 是 设备序列号
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13),//N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                Lat = 12.2,//C20 否   经度
                Lng = 71.2,//C20 否 纬度
                //Face：人脸识别 Eye：虹膜识别 Finger：指纹识别 Hand：掌纹识别 IDCard：身份证识别 RnCard：实名卡
                //Error：异常清退 Manuel：一键开闸 ExitChannel：应急通道  QRCode：二维码识别 App：APP考勤 Other：其他方式
                Mode = "Face",//C10 是   考勤方式： 
                OtherField = "test"//C200 否   扩展字段
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            //content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆考勤]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        #endregion

        #region 安全帽

        /// <summary>
        /// 获取安全帽识别ontent
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetHelmetContent()
        {
            //这是一个正式环境的密钥信息，测试已成功
            //_keySecret.SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002";
            //_keySecret.SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7";
            //_keySecret.ProjectKeyId = "f0f50da3-3cc7-49aa-950f-5788e61dc36c";
            //_keySecret.ProjectKeySecret = "ANQG5wj7S4kZXN0FhfNy9nxbGUQmckfw8ybZ";
            //var input = new List<object>();
            var input = new
            {
                serialNo = "50010310100001",
                serialNumber = "50010310100001",
                imageUrl = "http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080",
                smallImageUrl = "http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080",
                gatherTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            //input.Add(gate);
            var content = GetContent(input);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆安全帽]设备[{input.serialNo}]数据发送");
            return content;
        }


        /// <summary>
        /// 获取智慧工地安全帽预警数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetSafetyHelmetWarningContentV2()
        {
            var input = new
            {
                UnifiedProjectCode = _projectCode, //C50 是   工程项目编码
                DataTimeStamp = DateTime.Now.ToUnixTimeTicks(13), //N20 是   数据时间戳(示例精确到毫秒：1591231369050)
                DeviceFactory = "设备供应商", //C20 是   设备供应商
                DeviceSN = "TSVS837819828", //C50 是   设备序列号
                deviceCode = "服务器节点标识", //C100    是   服务器节点标识
                alarmType = 1, //C100    是   预警消息类型 1：人员入侵，2：安全帽预警
                generalInfo = new List<string> { "123", "456" }, //Array   是   总体数据(数组，包含：zoneId、recordTime、photoName、photoName、photoDocument)
                zoneId = "预警识别区域的标识", //C100    是   预警识别区域的标识
                photoName = "预警采集图片名称", //C100    是   格式为：yyyy - mm - dd hh:mm: ss
                photoDocument = Utilities.Base64Phto2.RemoveBase64ImagePrefix(), //C100    是 预警采集图片名称
                photoIdentify = "预警采集图片的MD5", //C100 是   图片文件(base64字符串并不包含前缀data: image / pngbase64，格式固定为.jpg或者.png)
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm"), //C100 是   预警采集图片的MD5
                OtherField = "test"//保留字段 //C200    否 保留字段
                                   //generalInfo = new{ 
                                   //    zoneId= "预警识别区域的标识", 
                                   //    recordTime= DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm")
                                   // },
            };
            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[重庆考勤]设备[{input.DeviceSN}]数据发送");
            return content;
        }

        #endregion

        #region 获取content

        private static StringContent GetContent(object input)
        {
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))}");
            var rCode = Utilities.GenerateRandomString(15);
            var ts = DateTime.Now.ToUnixTimestamp();
            var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
            var signature = Utilities.GetTokenBySupplierAndProject(rCode, ts, _keySecret?.SupplierKeySecret,
                _keySecret?.ProjectKeySecret);
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            content.Headers.Add("keyId", keyId);
            content.Headers.Add("ts", ts.ToString());
            content.Headers.Add("rCode", rCode);
            content.Headers.Add("signature", signature);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        private static async Task<StringContent> GetContentV2(object input)
        {
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            //var tokenResult = await GetAccessToken();
            //content.Headers.Add("Authorization", $"Bearer {tokenResult.Custom.Access_token}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        #endregion

        #region 城发

        /// <summary>
        /// 获取城发注册content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetChengFaRegisterContent()
        {
            var input = new List<object>();
            var register = new
            {
                pid = "268",
                operid = Guid.Empty,//73d09d6ec882401a9f9fabd178f4af79
                name = "测试",
                userimg = StringExtensions.UrlEncode("http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080"),
                gender = 1,
                birthday = "2019-01-01",
                age = "0",
                worktype = "测试",
                tel = "18923654258",
                idcard = "500108199003079558",
                entertime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                recordtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                estatus = 1,
                nation = "汉",
                //Provinces = "",
                //City = "",
                address = "重庆沙坪坝",
                fromwhere = "重庆沙坪坝",
                isfm = 1,
                companyId = "测试",
                //education = workerDevice.
                workTeamId = "测试",
                workTeamName = "测试",
                //fingerImg
                //fingerRemarks
                //laborLocation
                //laborArea
                //linkmanName
                //linkmanPhone
                //score
                //headIcon =
                //headIconRemarks
            };
            input.Add(register);
            //var text = TextJsonConvert.SerializeObject(new { data = input }) ?? "";
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))}");

            #region FormUrlEncodedContent

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("data", TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            });
            //var content = new MultipartFormDataContent
            //{
            //    { new StringContent(TextJsonConvert.SerializeObject(input)??""), "data" }
            //};

            #endregion


            #region MultipartFormDataContent

            //var content = new MultipartFormDataContent
            //{
            //    //{ new StringContent(TextJsonConvert.SerializeObject(workers)??""), "data" }
            //    { new StringContent(register.pid), "pid" },
            //    { new StringContent(register.operid.ToString()??""), "pid" },
            //    { new StringContent(register.name), "name" },
            //    { new StringContent(register.userimg), "userimg" },
            //    { new StringContent(register.gender.ToString()), "gender" },
            //    { new StringContent(register.birthday), "birthday" },
            //    { new StringContent(register.age), "age" },
            //    { new StringContent(register.worktype), "worktype" },
            //    { new StringContent(register.tel), "userimg" },
            //    { new StringContent(register.idcard), "userimg" },
            //    { new StringContent(register.entertime), "userimg" },
            //    { new StringContent(register.recordtime), "userimg" },
            //    { new StringContent(register.estatus.ToString()), "estatus" },
            //    { new StringContent(register.nation), "nation" },
            //    { new StringContent(register.address), "address" },
            //    { new StringContent(register.fromwhere), "fromwhere" },
            //    { new StringContent(register.isfm.ToString()), "isfm" },
            //    { new StringContent(register.companyId), "companyId" },
            //    { new StringContent(register.workTeamId), "workTeamId" },
            //    { new StringContent(register.workTeamName), "workTeamName" }
            //};
            //content.Headers.Add("ContentType", "multipart/form-data; boundary=<calculated when request is sent>");

            #endregion


            var rCode = Utilities.GenerateRandomString(10);
            var ts = DateTime.Now.ToUnixTimeTicks();
            content.Headers.Add("keyId", $"5adef286a9f94f75ba9488d617ce5caf");
            content.Headers.Add("ts", ts.ToString());
            content.Headers.Add("random", rCode);
            content.Headers.Add("signature", Utilities.GetTokenByProject(rCode, ts, "3d11f797bbd94513880fe4522c8b5333"));
            Console.WriteLine($"keyId:5adef286a9f94f75ba9488d617ce5caf");
            Console.WriteLine($"ts:{ts}");
            Console.WriteLine($"random:{rCode}");
            Console.WriteLine($"signature:{Utilities.GetTokenByProject(rCode, ts, "3d11f797bbd94513880fe4522c8b5333")}");
            return content;
        }



        #endregion


        #region 重庆建委同步人员接口V1

        /// <summary>
        /// 重庆建委同步人员接口V1
        /// </summary>
        /// <param name="serialNo"></param>
        /// <returns></returns>
        public static HttpContent GetSyncProjectWorkersV1Content(string serialNo)
        {
            //_keySecret.SupplierKeyId = "ea5cbc75-1f18-4c92-a449-023f6de0f002";
            //_keySecret.SupplierKeySecret = "igNd2gONoxWavfs4jfXR6SWcU8x6CdFnPep7";
            //_keySecret.ProjectKeyId = "f0f50da3-3cc7-49aa-950f-5788e61dc36c";
            //_keySecret.ProjectKeySecret = "ANQG5wj7S4kZXN0FhfNy9nxbGUQmckfw8ybZ";

            //var content = GetContent(input);
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("serialNo", serialNo),
                new KeyValuePair<string, string>("pageIndex", "0")
            });
            //var rCode = Utilities.GenerateRandomString(15);
            //var ts = DateTime.Now.ToUnixTimestamp();
            //var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
            //var signature = Utilities.GetTokenBySupplierAndProject(rCode, ts, _keySecret?.SupplierKeySecret,
            //    _keySecret?.ProjectKeySecret);

            //Console.WriteLine($"[重庆建委采集设备][{serialNo}]请求同步数据加密信息:");
            //content.Headers.Add("keyId", keyId);
            //content.Headers.Add("ts", ts.ToString());
            //content.Headers.Add("rCode", rCode);
            //content.Headers.Add("signature", signature);
            //Console.WriteLine($"keyId:{keyId}");
            //Console.WriteLine($"ts:{ts}");
            //Console.WriteLine($"random:{rCode}");
            //Console.WriteLine($"signature:{signature}");
            return content;
        }

        #endregion


        #region 帝馨车控接口



        /// <summary>
        /// 获取视频监控数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetVehicleBasicTokenContent()
        {
            var input = new
            {
                LoginName= "筑智建科技",//"筑智建科技"
                LoginPassword = "123456",
                LoginType = "ENTERPRISE",
                language = "cn",
                ISMD5 = "0",
                timeZone = "+08",
                apply = "APP",
                loginUrl = "http://vipapi.18gps.net/",
            };

            //var url =$"http://apitest.18gps.net/GetDateServices.asmx/loginSystem?LoginName={input.LoginName}&LoginPassword={input.LoginPassword}&LoginType=ENTERPRISE&language=cn&ISMD5=0&timeZone=+08&apply=APP&loginUrl=http://vipapi.18gps.net/";
            //Console.WriteLine(url);

            //var content = await GetContentV2(input);
            Console.WriteLine($"params:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(input)}");
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("params",TextJsonConvert.SerializeObject(input,new JsonSerializerOptions{PropertyNameCaseInsensitive = true}))
            };
            var content = new FormUrlEncodedContent(list);
            //content.Headers.Add("Authorization", $"Bearer {(await GetAccessToken())?.Custom.Access_token}");
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Console.WriteLine($"[筑智建科技]token请求发送");
            return content;
        }

        #endregion
    }

}
