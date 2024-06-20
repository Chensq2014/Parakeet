using ConsoleApp.Dtos;
using Common.Encrypts;
using Common.Extensions;
using System;
using Serilog;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApp.Area.Hunan
{
    /// <summary>
    /// 湖南测试
    /// </summary>
    public static class HunanTest
    {
        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret= new KeySecret
        {
            SupplierKeyId = "440742E5-AAE9-47F1-8931-C3B0393CE421",
            SupplierKeySecret = "2VMq6793Dv0os9p0oYee4LeYSCappXGKH8ez",
            ProjectKeyId = "5b73796d-7f86-46e3-b743-3d15dfe1ea86",
            ProjectKeySecret = "48efe90b03b947abae25a3ddad170af4"
        };

        /// <summary>
        /// 扬尘 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetEnvironmentContent()
        {
            var body = new
            {
                pm25 = "38.00",
                pm10 = "54.00",
                aqi = "68",
                temperature = "21.90",
                humidity = "49.40",
                windSpeed = "3.10",
                longitude = "125.6",
                latitude = "156.9",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南扬尘]设备[{body.deviceId}]数据发送");
            return content;
        }

        /// <summary>
        /// 考勤 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var body = new
            {
                unionNo="sdrfggdwsdwdrfgt",
                staffName="刘志鹏", 
                idCard= "4304111995",
                icCard="15978961abx", 
                staffType= "2",
                inOrOut= "1",
                photo= photo, 
                punchType= "1",
                punchTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),//"2019-08-13 02:12:33",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南考勤]设备[{body.deviceId}]数据发送");
            return content;
        }

        /// <summary>
        /// 塔吊基础信息 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneBasicContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var body = new
            {
                creanceId = "td-20191105",
                craneName = "塔吊-01",
                shortArm = 5,
                longArm = 75,
                craneHeight = 100,
                coordinate = "112.937421,28.235654", //坐标
                minCraneLoad = 0,
                maxCraneLoad = 100,
                minRatedKn = 10,
                maxRatedKn = 90,
                minAngle = 1,
                maxAngle = 20,
                minRadius = 0,
                maxRadius = 100,
                minHeight = 20,
                maxHeight = 100,
                minWindSpeed = 0,
                maxWindSpeed = 0,
                limitValue = 90,
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南塔吊]设备[{body.deviceId}]基础信息数据发送");
            return content;
        }

        /// <summary>
        /// 塔吊 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneContent()
        {
            var body = new
            {
                creanceId = "td-20191105",
                craneLoad = "10.3",
                ratedKn = "100.3",
                angle = "1",
                radius = "70",
                height = "100",
                windSpeed = "1.0",
                currentMaxLoad = "20.7",
                loadWarnState = "0",
                knWarnState = "0",
                //angleWarnState = "10.3",
                //radiusWarnState = "10.3",
                //heightWarnState = "10.3",
                //windSpeedWarnState = "10.3",
                rotationWarnState = "0",
                rotation = "10",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南塔吊]设备[{body.deviceId}]数据发送");
            return content;
        }

        /// <summary>
        /// 升降机基础数据 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterBasicContent()
        {
            var body = new
            {
                creanceId = "sjj-20200325",
                liftName = "升降机20200325",
                minLoad = "100.123",
                maxLoad = "123.123",
                minFloor = "1",
                maxFloor = "100",
                minHeight = "1",
                maxHeight = "100",
                minSpeed ="1",
                maxSpeed = "1100",
                minAngle = "1",
                maxAngle = "40",
                minWindSpeed = "1",
                maxWindSpeed = "100",
                minPeopleNum = "1",
                maxPeopleNum = "1000000",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南升降机]设备[{body.deviceId}]基础数据发送");
            return content;
        }

        /// <summary>
        /// 升降机 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterContent()
        {
            var body = new
            {
                creanceId = "sjj-20200325",
                liftName     = "升降机20200325",
                liftLoad      = "100.123",
                floor    = "1",
                height    = "1",
                speed     ="1",
                angle      = "1",
                radius     = "40",
                peopleNum = "1000000",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南升降机]设备[{body.deviceId}]数据发送");
            return content;
        }

        /// <summary>
        /// 升降机操作人员 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetLifterOperatorContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var body = new
            {
                creanceId = "sjj-20200325",
                staffName     = "升降机20200325",
                inOrOut      = "0",
                type       ="1",
                floor    = "1",
                idCard    = "1231896856",
                icCard    = "1231896856",
                speed     ="1",
                photo      = photo,
                coordinate     = "40.132,30.23161",
                specialCardNo = "1000000",
                effTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                unionNo="123654",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南升降机]设备[{body.deviceId}]操作人员数据发送");
            return content;
        }

        /// <summary>
        /// 劳务人员数据上传 Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetWorkerRegisterContent()
        {
            var photo = Utilities.Base64Phto.RemoveBase64ImagePrefix();//.UrlEncode();
            var body = new
            {
                staffName = "劳务人员A",
                unionNo = "yhuikyuky",
                isSpecial = false,
                idCard = "324324234",
                job = "工人",
                staffType = "1",
                sex = "1",
                photo = "http://39.108.81.42/static.jpg",
                companyNo = "10086",
                groupName = "234243",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南实名制]设备[{body.deviceId}]注册数据发送");
            return content;
        }

        /// <summary>
        /// 获取实噪声Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetNoiseContent()
        {
            var body = new
            {
                dbVal = "38.00",
                minVal = "6",
                maxVal = "60.5",
                deviceId = "wlw-dt100",
                recDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var content = AddContentHeaderParameters(body);

            Log.Logger.Debug($"[湖南噪声]设备[{body.deviceId}]数据发送");
            return content;
        }


        private static StringContent AddContentHeaderParameters(object body)
        {
            var json = Utilities.GetJsonSortByASCII(body);
            var sign = MD5Encrypt.Encrypt($"{_keySecret.ProjectKeyId}{json}{_keySecret.ProjectKeySecret}");
            var input = new
            {
                body = body,
                sign = sign,
                token = _keySecret.ProjectKeySecret
            };
            Log.Logger.Debug($"Json序列化input:");
            Log.Logger.Debug($"{JsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }


    }

}
