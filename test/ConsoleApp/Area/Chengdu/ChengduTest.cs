using System;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Dtos;
using Common.Extensions;

namespace ConsoleApp.Area.Chengdu
{
    /// <summary>
    /// 成都测试
    /// </summary>
    public static class ChengduTest
    {
        /// <summary>
        /// 加密信息
        /// </summary>
        private static readonly KeySecret _keySecret = new KeySecret
        {
            SupplierKeyId = "440742E5-AAE9-47F1-8931-C3B0393CE421",
            SupplierKeySecret = "2VMq6793Dv0os9p0oYee4LeYSCappXGKH8ez",
            ProjectKeyId = "440742E5-AAE9-47F1-8931-C3B0393CE427",
            ProjectKeySecret = "E3mjRc0LsJ7uKxQmIcs4wgDCmnubH3JajMLk"
        };

        /// <summary>
        /// 获取实名制注册Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetRegisterContent()
        {
            var encodePhoto = Utilities.Base64Phto.RemoveBase64ImagePrefix().UrlEncode();
            var input = new
            {
                idno = "500108199003079558",
                name = "智管京",
                gender = 1,
                nation = "汉",
                birthday = "1989-05-23",
                address = "重庆沙坪坝",
                idissue = "重庆沙坪坝公安局",
                idperiod = "20010101-20310101",
                userType = 1,
                dev_mac = "smz-cjkjg",
                regType = 3,
                phone = "18975213456",
                inf_photo = encodePhoto,
                idphoto = encodePhoto,
                photo = encodePhoto
            };
            var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("idno", input.idno),
                new KeyValuePair<string, string>("name", input.name),
                new KeyValuePair<string, string>("gender", input.gender.ToString()),
                new KeyValuePair<string, string>("nation", input.nation),
                new KeyValuePair<string, string>("birthday", input.birthday),
                new KeyValuePair<string, string>("address", input.address),
                new KeyValuePair<string, string>("idissue", input.idissue),
                new KeyValuePair<string, string>("idperiod", input.idperiod),
                new KeyValuePair<string, string>("userType", input.userType.ToString()),
                new KeyValuePair<string, string>("dev_mac", input.dev_mac),
                new KeyValuePair<string, string>("regType", input.regType.ToString()),
                new KeyValuePair<string, string>("phone", input.phone),
                new KeyValuePair<string, string>("inf_photo", input.inf_photo),
                new KeyValuePair<string, string>("idphoto", input.idphoto),
                new KeyValuePair<string, string>("photo", input.photo)
            };
            AddContentHeaderParameters(input);

            var content = new FormUrlEncodedContent(list);
            content.Headers.Add("ContentType", "application/x-www-form-urlencoded");
            Log.Logger.Debug($"[成都闸机实名制]设备[{input.dev_mac}]信息数据发送");
            return content;
        }

        /// <summary>
        /// 获取实名制注册Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetRegisterContentByFormData()
        {
            var encodePhoto = Utilities.Base64Phto3.RemoveBase64ImagePrefix().UrlEncode();
            var user = new
            {
                idno = "500108199003079558",
                name = "智管京",
                gender = 1,
                nation = "汉",
                birthday = "1989-05-23",
                address = "重庆沙坪坝",
                idissue = "重庆沙坪坝公安局",
                idperiod = "20010101-20310101",
                userType = 1,
                dev_mac = "smz-cjkjg",
                regType = 3,
                phone = "18975213456",
                inf_photo = encodePhoto,
                idphoto = encodePhoto,
                photo = encodePhoto
            };
            var content = new MultipartFormDataContent
            {
                //{ new StringContent(user.idno??""),"\"idno\""},
                //{ new StringContent(user.name??""),"\"name\""},
                //{ new StringContent(user.gender.ToString()),"\"gender\""},
                //{ new StringContent(user.nation??""),"\"nation\""},
                //{ new StringContent(user.birthday??""),"\"birthday\""},//.ToString("yyyy-MM-dd")
                //{ new StringContent(user.address??""),"\"address\""},
                //{ new StringContent(user.idissue??""),"\"idissue\""},
                //{ new StringContent(user.idperiod??""),"\"idperiod\""},
                //{ new StringContent(user.inf_photo??""),"\"inf_photo\""},
                //{ new StringContent(user.userType.ToString()),"\"userType\""},
                //{ new StringContent(user.dev_mac??""),"\"dev_mac\""},
                //{ new StringContent(user.regType.ToString()),"\"regType\""},
                //{ new StringContent(user.phone??""),"\"phone\""},
                //{ new StringContent(user.idphoto??""),"\"idphoto\""},//base64字符串
                //{ new StringContent(user.photo??""),"\"photo\""}//base64字符串
                { new StringContent(user.idno??""),"idno"},
                { new StringContent(user.name??""),"name"},
                { new StringContent(user.gender.ToString()),"gender"},
                { new StringContent(user.nation??""),"nation"},
                { new StringContent(user.birthday??""),"birthday"},//.ToString("yyyy-MM-dd")
                { new StringContent(user.address??""),"address"},
                { new StringContent(user.idissue??""),"idissue"},
                { new StringContent(user.idperiod??""),"idperiod"},
                { new StringContent(user.inf_photo??""),"inf_photo"},
                { new StringContent(user.userType.ToString()),"userType"},
                { new StringContent(user.dev_mac??""),"dev_mac"},
                { new StringContent(user.regType.ToString()),"regType"},
                { new StringContent(user.phone??""),"phone"},
                { new StringContent(user.idphoto??""),"idphoto"},//base64字符串
                { new StringContent(user.photo??""),"photo"}//base64字符串
            };
            AddContentHeaderParameters(user);
            content.Headers.Add("ContentType", "multipart/form-data; boundary=<calculated when request is sent>");
            return content;
        }

        /// <summary>
        /// 获取闸机
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetGateContent()
        {
            var logs = new List<object>();
            logs.Add(new
            {
                sn = "zzj-2020032701",
                user_id = "73d09d6ec882401a9f9fabd178f4af79",
                recog_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                recog_type = "iris"
            });
            var beforeEncryptData = new
            {
                count = 1,
                logs = logs
            };
            var json = TextJsonConvert.SerializeObject(beforeEncryptData);
            var input = new
            {
                sn = "zzj-2020032701",
                content = Utilities.Encrypt(json, "0dUeQ3SH")
            };
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sn",input.sn),
                new KeyValuePair<string, string>("content",input.content)
            });
            Log.Logger.Debug($"[成都闸机实名制]设备[{input.sn}]信息数据发送");
            return content;
        }


        /// <summary>
        /// 获取闸机
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetFeedBackContent()
        {
            var input = new
            {
                sn = "zzj-2020032701",
                type = 2.ToString(),
                msg = ""
            };
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sn",input.sn),
                new KeyValuePair<string, string>("type",input.type.ToString()),
                new KeyValuePair<string, string>("msg",input.msg)
            });
            Log.Logger.Debug($"[成都闸机实名制]设备[{input.sn}]操作反馈信息数据发送");
            AddContentHeaderParameters(input);
            return content;
        }


        /// <summary>
        /// 获取塔吊运行数据Content
        /// </summary>
        /// <returns></returns>
        public static HttpContent GetCraneRunContent()
        {
            #region 塔吊运行数据
            var craneRunArray = new List<object>();
            var craneRunData = new
            {
                time = DateTime.Now.ToUniversalTime(),
                angle = 12,
                extent = 25,
                weight = 125,
                safeWeight = 150,
                weightPer = 20,
                force = 1.2,
                forcePer = 20,
                height = 1.1,
                wind = 1.1,
                driverId = "510221125658996669",
                driverName = "test",
                power = 1,
                limitAlarmCode = 1,
                status = 1,
                obliqueAngleX = 1,
                obliqueAngleY = 2
            };
            craneRunArray.Add(craneRunData);
            var input = new
            {
                id = "NGTJ -001",
                data = craneRunArray
            };

            #endregion

            var content = AddContentHeaderParameters(input);
            SetCraneContentHeaderParameters(content);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Log.Logger.Debug($"[成都起重机]设备[{input.id}]基础信息数据发送");
            return content;
        }


        /// <summary>
        /// 设置塔机ContentHeader加密参数
        /// </summary>
        /// <param name="content"></param>
        private static void SetCraneContentHeaderParameters(StringContent content)
        {
            var rCode = Utilities.GenerateRandomString(15);
            var ts = DateTime.Now.ToUnixTimeTicks();
            var keyId = $"{_keySecret?.SupplierKeyId}_{_keySecret?.ProjectKeyId}";
            var signature = Utilities.GetTokenByProject(rCode, ts, _keySecret?.ProjectKeySecret);
            content.Headers.Add("keyId", keyId);
            content.Headers.Add("ts", ts.ToString());
            content.Headers.Add("rCode", rCode);
            content.Headers.Add("signature", signature);
        }


        private static StringContent AddContentHeaderParameters(object input)
        {
            Log.Logger.Debug($"Json序列化input:");
            Log.Logger.Debug($"input：{TextJsonConvert.DeserializeObject<object>(TextJsonConvert.SerializeObject(input))}");
            var content = new StringContent(TextJsonConvert.SerializeObject(input), Encoding.UTF8);
            return content;
        }


    }

}
