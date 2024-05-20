using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.HttpClients;
using Common.Dtos;
using Common.Extensions;

namespace ConsoleApp.Area.Cdceg
{
    /// <summary>
    /// 成都建工测试
    /// </summary>
    public static class CdcegTest
    {
        /// <summary>
        /// DevKey
        /// </summary>
        public static string DevKey = "e782816fba344ff489a9ab5583f5faf7";

        /// <summary>
        /// Host
        /// </summary>
        public static string Host = "http://www.ruimap.com";

        /// <summary>
        /// Port
        /// </summary>
        public static string Port = "8060";

        /// <summary>
        /// TokenApi
        /// </summary>
        public static string TokenApi = $@"/jgapi/devkey/verify";

        /// <summary>
        /// 获取EnvironmentContent
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetEnvironmentContent()
        {
            #region environment

            //var environment = new Dictionary<string, object>
            //{
            //    {"ST", ""},
            //    {"CN", ""},
            //    {"SK", ""},
            //    {"MN", "Y0028086400001"},
            //    {"DataTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
            //    {"PM10-Avg", 0},
            //    {"PM25-Avg", 0},
            //    {"T01-Avg", 0},
            //    {"B03-Avg", 0},
            //    {"H01-Avg", 0},
            //    {"W01-Avg", 0},
            //    {"W02-Avg", 0},
            //    {"R01-Avg", 0},
            //    {"TSP", 0},
            //    {"LNG", 0},
            //    {"LAT", 0}
            //};
            #endregion

            var content = new MultipartFormDataContent
            {
                {new StringContent(""), "ST"},
                {new StringContent(""), "CN"},
                {new StringContent(""), "SK"},
                {new StringContent("Y0028086400001"), "MN"},
                {new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), "DataTime"},
                {new StringContent(11.ToString()), "PM10-Avg"},
                {new StringContent(11.ToString()), "PM25-Avg"},
                {new StringContent(11.ToString()), "T01-Avg"},
                {new StringContent(11.ToString()), "B03-Avg"},
                {new StringContent(11.ToString()), "H01-Avg"},
                {new StringContent(11.ToString()), "W01-Avg"},
                {new StringContent(11.ToString()), "W02-Avg"},
                {new StringContent(11.ToString()), "R01-Avg"},
                {new StringContent(11.ToString()), "TSP"},
                {new StringContent(11.ToString()), "LNG"},
                {new StringContent(11.ToString()), "LAT"}
            };
            await AddHeaderParameters(content);
            return content;
        }

        /// <summary>
        /// 获取塔机Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetCraneContent()
        {
            var input = new
            {
                batterX = 0,
                batterY = 0,
                batteryLevel = 80,
                collectTime = DateTime.Now.Second,
                collideAlarmCode = 0000,
                electricRelayAlarmCode = 0000,
                height = 10,
                momentOfForce = 10,
                noEntryAlarmCode = 0000,
                obstacleAlarmCode = 0000,
                otherAlarmCode = 0000,
                rotation = 10,
                scope = 10,
                sn = "T0028086400001",
                spacingAlarmCode = 0000,
                weigh = 10,
                windSpeed = 10
            };
            Log.Logger.Debug($"Json序列化input对象");
            Log.Logger.Debug($"input：{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            await AddHeaderParameters(content);
            return content;
        }

        /// <summary>
        /// 获取实名制注册Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetRegisterContent()
        {
            var user = new
            {
                idno = "500108199003079558",
                name = "智管京",
                gender = 1.ToString(),
                nation = "汉",
                birthday = "1990-03-07",
                address = "重庆沙坪坝",
                idissue = "重庆沙坪坝公安局",
                idperiod = "20010101-20310101",
                inf_photo = Utilities.Base64Phto.RemoveBase64ImagePrefix(),
                userType = 1.ToString(),
                dev_mac = "SN510190_1001_12345_00015",
                regType = 3.ToString(),
                work_type = "主管",
                phone = "18975213456",
                admin_post = "主管",
                work_type_name = "主管",
                admin_post_name = "王大爷",
                idphoto = Utilities.Base64Phto.RemoveBase64ImagePrefix(),
                photo = Utilities.Base64Phto.RemoveBase64ImagePrefix()
            };
            var content = new MultipartFormDataContent
            {
                {new StringContent(user.idno),"idno"},
                {new StringContent(user.name),"name"},
                {new StringContent(user.gender.ToString()),"gender"},
                {new StringContent(user.nation),"nation"},
                {new StringContent(user.birthday),"birthday"},//.ToString("yyyy-MM-dd")
                {new StringContent(user.address),"address"},
                {new StringContent(user.idissue),"idissue"},
                {new StringContent(user.idperiod),"idperiod"},
                {new StringContent(user.inf_photo),"inf_photo"},
                {new StringContent(user.userType.ToString()),"userType"},
                {new StringContent(user.dev_mac),"dev_mac"},
                {new StringContent(user.regType.ToString()),"regType"},
                {new StringContent(user.work_type),"work_type"},
                {new StringContent(user.phone),"phone"},
                {new StringContent(user.admin_post),"admin_post"},
                {new StringContent(user.work_type_name),"work_type_name"},
                {new StringContent(user.admin_post_name),"admin_post_name"},
                {new StringContent(user.idphoto),"idphoto"},//base64字符串
                {new StringContent(user.photo),"photo"}//base64字符串
            };

            ////批量实名制数据采集
            //apiUrl = @"/jgapi/attendance/registBatch";
            //var users = new List<object>();
            //users.Add(user);
            //var content = new StringContent(new{users}, Encoding.UTF8);

            Log.Logger.Debug($"Json序列化input对象");
            Log.Logger.Debug($"input：{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(user))}");
            await AddHeaderParameters(content);
            content.Headers.Add("ContentType", $"multipart/form-data;charset=UTF-8");
            return content;
        }

        /// <summary>
        /// 获取视频Content
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpContent> GetVideoContent()
        {
            var input = new
            {
                sourceId = "a38611f8-ff08-40b7-95a1-f276df0f866e",
                serialNo = "TSVS837819829",
                showName = "项目监控",
                camNo = 1,
                coverUrl = "http://jwfs.typeo.org/a38611f8ff0840b795a1f276df0f866e.png",
                getUrl = "http://jwfs.typeo.org/api/v1/video/a38611f8-ff08-40b7-95a1-f276df0f866e",
                playUrl = "http://hls01open.ys7.com/openlive/a38611f8ff0840b795a1f276df0f866e.m3u8"
            };
            Log.Logger.Debug($"Json序列化input对象");
            Log.Logger.Debug($"input：{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            await AddHeaderParameters(content);
            return content;
        }


        /// <summary>
        /// 给content设置Header参数 共用
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static async Task<HttpContent> AddHeaderParameters(HttpContent content)
        {
            var tokenResult = await HttpClientPool.GetAccessToken(DevKey, $@"{Host}:{Port}", TokenApi);
            content.Headers.Add("AccessToken", tokenResult.AccessToken);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
    }

}
