using Common.Dtos;
using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.HttpClients;
using Newtonsoft.Json;
using Common.Extensions;

namespace ConsoleApp.Area.IOT
{
    /// <summary>
    /// IOT接口测试
    /// </summary>
    public static class IOTTest
    {

        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly AppTokenInputDto _tokenDto = new AppTokenInputDto
        {
            //AppId = "szhlgc",
            //AppKey = "iot_szhlgc",
            //AppSecret = "iot_72FAD5A47"

            AppId = "appId",
            AppKey = "appKey",
            AppSecret = "appSecret"

            //AppId = "archglId",
            //AppKey = "archglKey",
            //AppSecret = "archglSecret",

            //AppId = "kczx",//"archglId",
            //AppKey = "device",//"archglKey",
            //AppSecret = "iot_jA9HFJQz",//"archglSecret",

            //AppId = "archglId",
            //AppKey = "archglKey",
            //AppSecret = "archglSecret"
            //TimeStamp = ((DateTime.Now.Ticks - 621355968000000000) / 10000000).ToString()
        };


        /// <summary>
        /// 设置appId appKey appSecret
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <returns></returns>
        public static AppTokenInputDto UpdateTokenDto(string appId,string appKey,string appSecret)
        {
            _tokenDto.AppId = appId;
            _tokenDto.AppKey = appKey;
            _tokenDto.AppSecret = appSecret;
            return _tokenDto;
        }

        /// <summary>
        /// 获取闸机考勤
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetGateContent()
        {
            var input = new
            {
                name = "测试考勤",
                idCard = "500223198905291751",
                personnelId = "3fa85f6457174562b3fc2c963f66afa6",
                photo = Utilities.Base64Phto2,
                inOrOut = 1,
                photoUrl = "string",
                workerNo = "string",
                recogType = "3",
                icCard = "string",
                personnelType = "1",
                serialNo = "99999910050001",//"50010810050001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[考勤]设备[{input.serialNo}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取环境数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentContent()
        {
            #region 环境数据
            var input = new
            {
                pM2P5 = 0,
                pM10 = 0,
                tsp = 0,
                noise = 0,
                temperature = 0,
                humidity = 0,
                windDirection = 0,
                windSpeed = 0,
                rainfall = 0,
                pressure = 0,
                tspFlag = "string",
                pM2P5Flag = "string",
                pM10Flag = "string",
                noiseFlag = "string",
                windDirectionFlag = "string",
                windSpeedFlag = "string",
                temperatureFlag = "string",
                humidityFlag = "string",
                pressureFlag = "string",
                rainfallFlag = "string",
                serialNo = "1234569999",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            #endregion

            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[环境]设备[{input.serialNo}]基础信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取塔吊基础数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneBasicContent()
        {
            var input = new
            {
                serialNo = "99999910030001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[塔吊]设备[{input.serialNo}]基础数据发送");
            return content;
        }


        /// <summary>
        /// 注册人员Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetWorkerRegisterContent()
        {
            var input = new
            {
                name = "测试注册人员",
                idCard = "500108199003079558",
                gender = 1,
                nation = "string",
                birthday = "2020-08-14",
                address = "string",
                issuedBy = "string",
                termValidityStart = "20200814",
                termValidityEnd = "20200831",
                //idPhoto= "string",
                photo = Utilities.Base64Phto2,
                phoneNumber = "string",
                icCard = "500108199003079558",
                personnelId = "3fa85f6457174562b3fc2c963f66afa6",
                personnelType = 1,
                registerType = 3,
                idPhotoUrl = "http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080",
                photoUrl = "http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080",
                infraredPhotoUrl = "http://zzj-iot-image-test.oss-cn-chengdu.aliyuncs.com/test123456?x-oss-process=image/resize,w_1080",
                serialNo = "99999910050003",
                //serialNo = "35021210050001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                //serialNo= "35021210050001",
                //name= "贺先生",
                //idCard= "430529198806150123",
                //gender= 2,
                //nation= "汉族",
                //birthday= "1988-06-15",
                //address= "11",
                //issuedBy= "公安局",
                //termValidityStart= "2015922",
                //termValidityEnd= "2035922",
                //phoneNumber= "18112312341",
                //icCard= "123456",
                //personnelId= "c11db2a5f4364f679598c4e6c06926c7",
                //personnelType= 1,
                //registerType= 3,
                //GroupLeader= false,
                //Marital= "已婚",
                //LaborCompanyName= "福建成森建设集团有限公司",
                //WorkerTypeCode= "010",
                //WorkerGroupName= "钢筋班组"
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[闸机采集]设备[{input.serialNo}]数据发送");
            return content;
        }



        /// <summary>
        /// 获取塔吊运行数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneRecordContent()
        {
            var input = new
            {
                craneId = 0,
                height = 0,
                range = 0,
                rotation = 0,
                load = 0,
                windSpeed = 0,
                tiltAngle = 0,
                torque = 0,
                torquePercent = 0,
                fall = 0,
                weightPercent = 0,
                safeLoad = 0,
                obliquity = 0,
                dirAngle = 0,
                message = "string",
                idCard = "string",
                driverName = "string",
                powerStatus = 0,
                status = 0,
                tiltX = 0,
                tiltY = 0,
                alarmCode = 0,
                loadWarnState = 0,
                knWarnState = 0,
                angleWarnState = 0,
                radiusWarnState = 0,
                heightWarnState = 0,
                windSpeedWarnState = 0,
                rotationWarnState = 0,
                serialNo = "99999910030001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[重庆塔吊]设备[{input.serialNo}]运行数据发送");
            return content;
        }



        /// <summary>
        /// 获取塔吊工作数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneCycleContent()
        {
            var input = new
            {
                craneId = 0,
                startTime = "2020-08-13T07:25:51.056Z",
                endTime = "2020-09-13T07:25:51.056Z",
                maxWeight = 0,
                maxTorque = 0,
                maxHeight = 0,
                minHeight = 0,
                maxRange = 0,
                minRange = 0,
                minTitl = 0,
                maxTitl = 0,
                maxTorquePercent = 0,
                maxWindSpeed = 0,
                loadPointAngle = 0,
                loadPointRange = 0,
                loadPointHeight = 0,
                unloadPointAngle = 0,
                unloadPointRange = 0,
                unloadPointHeight = 0,
                status = 0,
                idCard = "string",
                driverName = "string",
                powerStatus = 0,
                hasDriver = true,
                serialNo = "99999910030001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[塔吊]设备[{input.serialNo}]工作数据发送");
            return content;
        }


        /// <summary>
        /// 获取环境数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetHeartbeatContent(string serialNo)
        {
            #region 心跳数据
            var input = new
            {
                serialNo = serialNo,//"1234569999",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            #endregion

            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"设备[{input.serialNo}]心跳数据发送");
            return content;
        }


        /// <summary>
        /// 获取基坑数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetFoundationpitContent()
        {
            #region 基坑数据

            var records = new List<object> {new
            {
                PointNumber=1,
                BasePoint=false,
                Eastward=12.0,
                Northward=12.0,
                Verticalward=12.0
            }};
            var input = new
            {
                SerialNo = "1234569999",
                TerminalId = 1,
                SequenceNo = DateTime.Now.ToUnixInt(),
                RecordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Records= records
            };

            #endregion

            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //Log.Logger.Debug($"[基坑]设备[{input.SerialNo}]基础信息数据发送");
            Console.WriteLine($"[基坑]设备[{input.SerialNo}]基础信息数据发送");
            return content;
        }





        /// <summary>
        /// 获取水质检测数据Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetWaterQualityContent()
        {
            var input = new
            {
                serialNo = "50010630210001",
                recordTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(input);
            await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[沙区管网]设备[{input.serialNo}]基础数据发送");
            return content;
        }

        /// <summary>
        /// 获取空Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetRequestWithTokenContent(string token)
        {
            var input = new
            {
            };
            var content = AddContentHeaderParameters(input);
            //await SetIOTContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //content.Headers.Add("Authorization", $"Bearer {token}");
            Log.Logger.Debug($"空数据发送");
            return content;
        }
        /// <summary>
        /// 获取iot_ops token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetIotOpsToken()
        {
            //https://ops.spdyun.cn
            return await HttpClientPool.GetIotOpsToken("https://localhost:5001/connect/token", "admin", "1q2w3E*",
                "DataCenterWebApp", "1q2w3E*","password", "email openid profile role phone address DataCenter");
        }

        /// <summary>
        /// 设置塔机ContentHeader加密参数
        /// </summary>
        /// <param name="content"></param>
        private static async Task SetIOTContentHeaderParameters(StringContent content)
        {
            _tokenDto.TimeStamp = DateTime.Now.ToUnixTimeTicks().ToString();
            await Utilities.GetIotToken(_tokenDto);
            content.Headers.Add("AppId", _tokenDto.AppId);
            content.Headers.Add("AppKey", _tokenDto.AppKey);
            content.Headers.Add("AppToken", _tokenDto.Token);
            content.Headers.Add("TimeStamp", _tokenDto.TimeStamp);

            Console.WriteLine($"AppId:{_tokenDto.AppId}");
            Console.WriteLine($"AppKey:{_tokenDto.AppKey}");
            Console.WriteLine($"AppToken:{_tokenDto.Token}");
            Console.WriteLine($"TimeStamp:{_tokenDto.TimeStamp}");
        }


        private static StringContent AddContentHeaderParameters(object input)
        {
            //Log.Logger.Information($"Json序列化input:");
            Console.WriteLine($"Json序列化input:");
            Console.WriteLine($"{Newtonsoft.Json.JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            return content;
        }
    }

}
